using MediatR;
using SistemaGestaoEscolar.Alunos.Aplicacao.Queries;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Dominio.Entidades;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.QueryHandlers;

public class ObterAlunosPorTurmaQueryHandler : IRequestHandler<ObterAlunosPorTurmaQuery, ObterAlunosPorTurmaResponse>
{
    private readonly IRepositorioMatricula _repositorioMatricula;
    private readonly IRepositorioAluno _repositorioAluno;

    public ObterAlunosPorTurmaQueryHandler(
        IRepositorioMatricula repositorioMatricula,
        IRepositorioAluno repositorioAluno)
    {
        _repositorioMatricula = repositorioMatricula ?? throw new ArgumentNullException(nameof(repositorioMatricula));
        _repositorioAluno = repositorioAluno ?? throw new ArgumentNullException(nameof(repositorioAluno));
    }

    public async Task<ObterAlunosPorTurmaResponse> Handle(ObterAlunosPorTurmaQuery request, CancellationToken cancellationToken)
    {
        var response = new ObterAlunosPorTurmaResponse();

        try
        {
            if (request.TurmaId == Guid.Empty)
            {
                response.Erros.Add("ID da turma é obrigatório");
                response.Sucesso = false;
                return response;
            }

            // Definir ano letivo se não fornecido
            var anoLetivo = request.AnoLetivo ?? DateTime.Now.Year;

            // Buscar matrículas da turma
            var matriculas = await _repositorioMatricula.ObterPorTurmaAsync(request.TurmaId);
            
            // Filtrar por ano letivo
            matriculas = matriculas.Where(m => m.AnoLetivo == anoLetivo);
            
            // Filtrar por status se necessário
            if (request.ApenasAtivos)
            {
                matriculas = matriculas.Where(m => m.EstaAtiva());
            }

            var matriculasList = matriculas.ToList();

            // Buscar alunos das matrículas
            var alunosIds = matriculasList.Select(m => m.AlunoId).ToList();
            var alunosTasks = alunosIds.Select(id => _repositorioAluno.ObterComResponsaveisPorIdAsync(id));
            var alunosArray = await Task.WhenAll(alunosTasks);
            var alunos = alunosArray.Where(a => a != null).ToList();

            // Aplicar filtro de nome se fornecido
            if (!string.IsNullOrWhiteSpace(request.FiltroNome))
            {
                alunos = alunos.Where(a => a!.Nome.ContemNome(request.FiltroNome)).ToList();
            }

            // Mapear para DTOs
            var alunosDto = new List<AlunoTurmaDto>();
            
            foreach (var aluno in alunos)
            {
                if (aluno == null) continue;
                
                var matricula = matriculasList.FirstOrDefault(m => m.AlunoId == aluno.Id);
                if (matricula == null) continue;

                var alunoDto = new AlunoTurmaDto
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome.Valor,
                    Cpf = aluno.Cpf.NumeroFormatado,
                    DataNascimento = aluno.DataNascimento.Valor,
                    Idade = aluno.DataNascimento.Idade,
                    Telefone = aluno.Telefone,
                    Email = aluno.Email,
                    Ativo = aluno.Ativo,
                    MatriculaId = matricula.Id,
                    NumeroMatricula = matricula.NumeroMatricula,
                    DataMatricula = matricula.DataMatricula,
                    StatusMatricula = matricula.Status.ObterDescricao(),
                    Responsaveis = aluno.Responsaveis.Select(r => new ResponsavelResumoDto
                    {
                        Id = r.Id,
                        Nome = r.Nome.Valor,
                        Telefone = r.Telefone,
                        Email = r.Email,
                        TipoDescricao = r.Tipo.ObterDescricao(),
                        ResponsavelFinanceiro = r.ResponsavelFinanceiro,
                        ResponsavelAcademico = r.ResponsavelAcademico
                    }).ToList()
                };

                alunosDto.Add(alunoDto);
            }

            // Ordenar por nome
            alunosDto = alunosDto.OrderBy(a => a.Nome).ToList();

            // Preparar resposta
            response.Alunos = alunosDto;
            response.TurmaId = request.TurmaId;
            response.NomeTurma = "Nome da Turma"; // Seria obtido via integração com módulo Acadêmico
            response.AnoLetivo = anoLetivo;
            response.TotalAlunos = alunosDto.Count;
            response.AlunosAtivos = alunosDto.Count(a => a.Ativo);
            response.AlunosInativos = alunosDto.Count(a => !a.Ativo);
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
}