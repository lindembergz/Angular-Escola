using MediatR;
using SistemaGestaoEscolar.Alunos.Aplicacao.Queries;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Dominio.Servicos;
using SistemaGestaoEscolar.Alunos.Dominio.Entidades;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.QueryHandlers;

public class ObterAlunoDetalheQueryHandler : IRequestHandler<ObterAlunoDetalheQuery, ObterAlunoDetalheResponse>
{
    private readonly IRepositorioAluno _repositorioAluno;
    private readonly IServicosDominioAluno _servicosDominio;

    public ObterAlunoDetalheQueryHandler(
        IRepositorioAluno repositorioAluno,
        IServicosDominioAluno servicosDominio)
    {
        _repositorioAluno = repositorioAluno ?? throw new ArgumentNullException(nameof(repositorioAluno));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
    }

    public async Task<ObterAlunoDetalheResponse> Handle(ObterAlunoDetalheQuery request, CancellationToken cancellationToken)
    {
        var response = new ObterAlunoDetalheResponse();

        try
        {
            if (request.Id == Guid.Empty)
            {
                response.Erros.Add("ID do aluno é obrigatório");
                response.Sucesso = false;
                return response;
            }

            // Buscar aluno com relacionamentos
            var aluno = await _repositorioAluno.ObterCompletoAsync(request.Id);
            if (aluno == null)
            {
                response.Erros.Add("Aluno não encontrado");
                response.Sucesso = false;
                return response;
            }

            // Obter pendências do aluno
            var pendencias = _servicosDominio.ObterPendenciasDocumentais(aluno);

            // Mapear para DTO
            response.Aluno = await MapearParaDetalhe(aluno, pendencias.ToList());
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

    private static async Task<AlunoDetalheDto> MapearParaDetalhe(
        SistemaGestaoEscolar.Alunos.Dominio.Entidades.Aluno aluno, 
        List<string> pendencias)
    {
        var dto = new AlunoDetalheDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome.Valor,
            Cpf = aluno.Cpf.NumeroFormatado,
            DataNascimento = aluno.DataNascimento.Valor,
            Idade = aluno.DataNascimento.Idade,
            FaixaEtariaEscolar = aluno.DataNascimento.ObterFaixaEtariaEscolar(),
            Endereco = new EnderecoDto
            {
                Logradouro = aluno.Endereco.Logradouro,
                Numero = aluno.Endereco.Numero,
                Complemento = aluno.Endereco.Complemento,
                Bairro = aluno.Endereco.Bairro,
                Cidade = aluno.Endereco.Cidade,
                Estado = aluno.Endereco.Estado,
                Cep = aluno.Endereco.Cep,
                CepFormatado = aluno.Endereco.CepFormatado,
                EnderecoCompleto = aluno.Endereco.EnderecoCompleto
            },
            Genero = new GeneroDto
            {
                Tipo = (int)aluno.Genero.Tipo,
                Descricao = aluno.Genero.ToString()
            },
            Deficiencia = new DeficienciaDto
            {
                Tipo = aluno.Deficiencia.PossuiDeficiencia ? (int)aluno.Deficiencia.Tipo! : null,
                TipoDescricao = aluno.Deficiencia.PossuiDeficiencia ? aluno.Deficiencia.Tipo!.ToString() : null,
                Descricao = aluno.Deficiencia.Descricao,
                PossuiDeficiencia = aluno.Deficiencia.PossuiDeficiencia
            },
            Telefone = aluno.Telefone,
            Email = aluno.Email,
            Observacoes = aluno.Observacoes,
            EscolaId = aluno.EscolaId,
            NomeEscola = "Nome da Escola", // Seria obtido via integração com módulo Escolas
            DataCadastro = aluno.DataCadastro,
            Ativo = aluno.Ativo,
            Pendencias = pendencias
        };

        // Mapear responsáveis
        dto.Responsaveis = aluno.Responsaveis.Select(r => new ResponsavelDto
        {
            Id = r.Id,
            Nome = r.Nome.Valor,
            Cpf = r.Cpf.NumeroFormatado,
            Telefone = r.Telefone,
            Email = r.Email,
            TipoDescricao = r.Tipo.ObterDescricao(),
            Profissao = r.Profissao,
            LocalTrabalho = r.LocalTrabalho,
            TelefoneTrabalho = r.TelefoneTrabalho,
            ResponsavelFinanceiro = r.ResponsavelFinanceiro,
            ResponsavelAcademico = r.ResponsavelAcademico,
            AutorizadoBuscar = r.AutorizadoBuscar,
            Endereco = r.Endereco != null ? new EnderecoDto
            {
                Logradouro = r.Endereco.Logradouro,
                Numero = r.Endereco.Numero,
                Complemento = r.Endereco.Complemento,
                Bairro = r.Endereco.Bairro,
                Cidade = r.Endereco.Cidade,
                Estado = r.Endereco.Estado,
                Cep = r.Endereco.Cep,
                CepFormatado = r.Endereco.CepFormatado,
                EnderecoCompleto = r.Endereco.EnderecoCompleto
            } : null
        }).ToList();

        // Mapear matrículas
        dto.Matriculas = aluno.Matriculas.Select(m => new MatriculaDto
        {
            Id = m.Id,
            TurmaId = m.TurmaId,
            NomeTurma = "Nome da Turma", // Seria obtido via integração com módulo Acadêmico
            AnoLetivo = m.AnoLetivo,
            NumeroMatricula = m.NumeroMatricula,
            DataMatricula = m.DataMatricula,
            DataCancelamento = m.DataCancelamento,
            MotivoCancelamento = m.MotivoCancelamento,
            Ativa = m.Ativa,
            StatusDescricao = m.Status.ObterDescricao(),
            Observacoes = m.Observacoes,
            DiasMatriculado = m.DiasMatriculado()
        }).ToList();

        // Matrícula ativa
        var matriculaAtiva = aluno.ObterMatriculaAtiva();
        if (matriculaAtiva != null)
        {
            dto.MatriculaAtiva = dto.Matriculas.FirstOrDefault(m => m.Id == matriculaAtiva.Id);
        }

        await Task.CompletedTask;
        return dto;
    }
}