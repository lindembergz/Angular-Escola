using MediatR;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;
using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Dominio.Servicos;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.CommandHandlers;

public class MatricularAlunoCommandHandler : IRequestHandler<MatricularAlunoCommand, MatricularAlunoResponse>
{
    private readonly IRepositorioAluno _repositorioAluno;
    private readonly IRepositorioMatricula _repositorioMatricula;
    private readonly IServicosDominioAluno _servicosDominio;

    public MatricularAlunoCommandHandler(
        IRepositorioAluno repositorioAluno,
        IRepositorioMatricula repositorioMatricula,
        IServicosDominioAluno servicosDominio)
    {
        _repositorioAluno = repositorioAluno ?? throw new ArgumentNullException(nameof(repositorioAluno));
        _repositorioMatricula = repositorioMatricula ?? throw new ArgumentNullException(nameof(repositorioMatricula));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
    }

    public async Task<MatricularAlunoResponse> Handle(MatricularAlunoCommand request, CancellationToken cancellationToken)
    {
        var response = new MatricularAlunoResponse();

        try
        {
            // Buscar aluno existente
            var aluno = await _repositorioAluno.ObterComMatriculasPorIdAsync(request.AlunoId);

            if (aluno == null)
            {
                response.Erros.Add("Aluno não encontrado");
                response.Sucesso = false;
                return response;
            }

            // Validar dados
            var errosValidacao = await ValidarCommand(request);
            if (errosValidacao.Any())
            {
                response.Erros = errosValidacao.ToList();
                response.Sucesso = false;
                return response;
            }

            // Verificar se pode matricular (usando repositório diretamente na camada de aplicação)
            if (await _repositorioMatricula.ExisteMatriculaAtivaAsync(request.AlunoId, request.AnoLetivo))
            {
                response.Erros.Add("Aluno já possui matrícula ativa para este ano letivo");
                response.Sucesso = false;
                return response;
            }

            // Verificar capacidade da turma
            var capacidadeDisponivel = await _repositorioMatricula.ObterCapacidadeDisponivelTurmaAsync(request.TurmaId, request.AnoLetivo);
            if (capacidadeDisponivel <= 0)
            {
                response.Erros.Add("Turma não possui vagas disponíveis");
                response.Sucesso = false;
                return response;
            }

            // Validar regras de negócio da matrícula (usando serviços puros)
            var errosMatricula = _servicosDominio.ValidarRegrasNegocioMatricula(aluno, request.AnoLetivo);
            if (errosMatricula.Any())
            {
                response.Erros.AddRange(errosMatricula);
                response.Sucesso = false;
                return response;
            }

            // Criar matrícula
            var matricula = new Matricula(
                request.AlunoId,
                request.TurmaId,
                request.AnoLetivo,
                request.Observacoes);

            // Adicionar matrícula ao aluno
            aluno.AdicionarMatricula(matricula);

            // Salvar matrícula
            await _repositorioMatricula.AdicionarAsync(matricula);
            await _repositorioAluno.AtualizarAsync(aluno);

            // TODO: Notificar responsáveis sobre a matrícula
            // Esta funcionalidade será implementada via Event Handlers

            // Preparar resposta de sucesso
            response.MatriculaId = matricula.Id;
            response.AlunoId = aluno.Id;
            response.NomeAluno = aluno.Nome.Valor;
            response.TurmaId = request.TurmaId;
            response.AnoLetivo = request.AnoLetivo;
            response.NumeroMatricula = matricula.NumeroMatricula;
            response.DataMatricula = matricula.DataMatricula;
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

    private static async Task<IEnumerable<string>> ValidarCommand(MatricularAlunoCommand command)
    {
        var erros = new List<string>();

        if (command.AlunoId == Guid.Empty)
            erros.Add("ID do aluno é obrigatório");

        if (command.TurmaId == Guid.Empty)
            erros.Add("ID da turma é obrigatório");

        if (command.AnoLetivo <= 0)
            erros.Add("Ano letivo é obrigatório");

        var anoAtual = DateTime.Now.Year;
        if (command.AnoLetivo < anoAtual - 1 || command.AnoLetivo > anoAtual + 1)
            erros.Add($"Ano letivo deve estar entre {anoAtual - 1} e {anoAtual + 1}");

        await Task.CompletedTask;
        return erros;
    }
}