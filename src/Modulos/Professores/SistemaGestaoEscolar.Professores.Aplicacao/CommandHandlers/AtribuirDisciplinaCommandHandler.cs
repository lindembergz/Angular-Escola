using MediatR;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;
using SistemaGestaoEscolar.Professores.Dominio.Servicos;

namespace SistemaGestaoEscolar.Professores.Aplicacao.CommandHandlers;

public class AtribuirDisciplinaCommandHandler : IRequestHandler<AtribuirDisciplinaCommand, Unit>
{
    private readonly IRepositorioProfessor _repositorio;
    private readonly IServicosDominioProfessor _servicosDominio;
    private readonly IServicoIntegracaoDisciplina _servicoIntegracaoDisciplina;
    private readonly ILogger<AtribuirDisciplinaCommandHandler> _logger;

    public AtribuirDisciplinaCommandHandler(
        IRepositorioProfessor repositorio,
        IServicosDominioProfessor servicosDominio,
        IServicoIntegracaoDisciplina servicoIntegracaoDisciplina,
        ILogger<AtribuirDisciplinaCommandHandler> logger)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
        _servicoIntegracaoDisciplina = servicoIntegracaoDisciplina ?? throw new ArgumentNullException(nameof(servicoIntegracaoDisciplina));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(AtribuirDisciplinaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atribuição de disciplina {DisciplinaId} ao professor {ProfessorId}", 
            request.DisciplinaId, request.ProfessorId);

        // Buscar o professor
        var professor = await _repositorio.ObterPorIdAsync(request.ProfessorId);
        if (professor == null)
        {
            _logger.LogWarning("Professor {ProfessorId} não encontrado", request.ProfessorId);
            throw new InvalidOperationException("Professor não encontrado");
        }

        // Validar se a disciplina existe e está ativa
        var disciplinaExiste = await _servicoIntegracaoDisciplina.DisciplinaExisteAsync(request.DisciplinaId);
        if (!disciplinaExiste)
        {
            _logger.LogWarning("Disciplina {DisciplinaId} não existe ou está inativa", request.DisciplinaId);
            throw new InvalidOperationException("Disciplina não encontrada ou inativa");
        }

        // Validar se o professor pode lecionar esta disciplina
        var podeAtribuir = await _servicoIntegracaoDisciplina.PodeLecionar(request.ProfessorId, request.DisciplinaId);
        if (!podeAtribuir)
        {
            _logger.LogWarning("Professor {ProfessorId} não pode lecionar disciplina {DisciplinaId}", 
                request.ProfessorId, request.DisciplinaId);
            throw new InvalidOperationException("Professor não pode lecionar esta disciplina");
        }

        // Validações adicionais do domínio
        var podeAtribuirDominio = await _servicosDominio.PodeAtribuirDisciplinaAsync(request.ProfessorId, request.DisciplinaId);
        if (!podeAtribuirDominio)
        {
            _logger.LogWarning("Validação de domínio falhou para atribuição de disciplina {DisciplinaId} ao professor {ProfessorId}", 
                request.DisciplinaId, request.ProfessorId);
            throw new InvalidOperationException("Não é possível atribuir esta disciplina ao professor");
        }

        // Atribuir a disciplina
        professor.AtribuirDisciplina(request.DisciplinaId, request.CargaHorariaSemanal, request.Observacoes);

        await _repositorio.AtualizarAsync(professor);

        _logger.LogInformation("Disciplina {DisciplinaId} atribuída com sucesso ao professor {ProfessorId}", 
            request.DisciplinaId, request.ProfessorId);

        return Unit.Value;
    }
}