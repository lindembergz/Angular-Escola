using MediatR;
using SistemaGestaoEscolar.Alunos.Aplicacao.Queries;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.QueryHandlers;

public class ObterAlunosQueryHandler : IRequestHandler<ObterAlunosQuery, ObterAlunosResponse>
{
    private readonly IRepositorioAluno _repositorioAluno;

    public ObterAlunosQueryHandler(IRepositorioAluno repositorioAluno)
    {
        _repositorioAluno = repositorioAluno ?? throw new ArgumentNullException(nameof(repositorioAluno));
    }

    public async Task<ObterAlunosResponse> Handle(ObterAlunosQuery request, CancellationToken cancellationToken)
    {
        var response = new ObterAlunosResponse();

        try
        {
            // Validar parâmetros
            if (request.Page <= 0)
                request.Page = 1;

            if (request.PageSize <= 0 || request.PageSize > 100)
                request.PageSize = 50;

            // Buscar alunos com filtros
            IEnumerable<SistemaGestaoEscolar.Alunos.Dominio.Entidades.Aluno> alunos;

            if (HasAdvancedFilters(request))
            {
                // Usar busca avançada
                alunos = await _repositorioAluno.BuscarAvancadaAsync(
                    nome: request.FiltroNome,
                    escolaId: request.FiltroEscola,
                    idadeMinima: request.IdadeMinima,
                    idadeMaxima: request.IdadeMaxima,
                    cidade: request.FiltroCidade,
                    estado: request.FiltroEstado,
                    ativo: request.FiltroAtivo,
                    possuiMatriculaAtiva: request.PossuiMatriculaAtiva);
            }
            else
            {
                // Usar busca paginada simples
                var resultado = await _repositorioAluno.ObterPaginadoAsync(
                    request.Pagina,
                    request.TamanhoPagina,
                    request.FiltroNome,
                    request.FiltroEscola,
                    request.FiltroAtivo);

                response.TotalRegistros = resultado.Total;
                alunos = resultado.Alunos;
            }

            // Se usou busca avançada, aplicar paginação manualmente
            if (HasAdvancedFilters(request))
            {
                var alunosList = alunos.ToList();
                response.TotalRegistros = alunosList.Count;
                
                alunos = alunosList
                    .Skip((request.Pagina - 1) * request.TamanhoPagina)
                    .Take(request.TamanhoPagina);
            }

            // Converter para DTOs
            response.Alunos = alunos.Select(MapearParaResumo).ToList();
            response.PaginaAtual = request.Pagina;
            response.TamanhoPagina = request.TamanhoPagina;
            response.TotalPaginas = (int)Math.Ceiling((double)response.TotalRegistros / request.TamanhoPagina);
            response.Sucesso = true;

            return response;
        }
        catch (Exception ex)
        {
            response.Erros.Add($"Erro interno: {ex.Message}");
            response.Sucesso = false;
            return response;
        }
    }

    private static bool HasAdvancedFilters(ObterAlunosQuery request)
    {
        return request.IdadeMinima.HasValue ||
               request.IdadeMaxima.HasValue ||
               !string.IsNullOrEmpty(request.FiltroCidade) ||
               !string.IsNullOrEmpty(request.FiltroEstado) ||
               request.PossuiMatriculaAtiva.HasValue;
    }

    private static AlunoResumoDto MapearParaResumo(SistemaGestaoEscolar.Alunos.Dominio.Entidades.Aluno aluno)
    {
        return new AlunoResumoDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome.Valor,
            Cpf = aluno.Cpf.NumeroFormatado,
            DataNascimento = aluno.DataNascimento.Valor,
            Idade = aluno.DataNascimento.Idade,
            GeneroDescricao = aluno.Genero.ToString(),
            PossuiDeficiencia = aluno.Deficiencia.PossuiDeficiencia,
            DeficienciaDescricao = aluno.Deficiencia.PossuiDeficiencia ? aluno.Deficiencia.ToString() : null,
            Telefone = aluno.Telefone ?? "",
            Email = aluno.Email ?? "",
            Cidade = aluno.Endereco.Cidade,
            Estado = aluno.Endereco.Estado,
            Ativo = aluno.Ativo,
            PossuiMatriculaAtiva = aluno.PossuiMatriculaAtiva(),
            NomeTurmaAtual = aluno.ObterMatriculaAtiva()?.TurmaId.ToString(), // Seria obtido via join
            QuantidadeResponsaveis = aluno.Responsaveis.Count,
            DataCadastro = aluno.DataCadastro
        };
    }
}