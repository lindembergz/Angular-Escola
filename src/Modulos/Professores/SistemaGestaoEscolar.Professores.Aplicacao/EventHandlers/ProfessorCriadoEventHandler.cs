using MediatR;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Professores.Dominio.Eventos;

namespace SistemaGestaoEscolar.Professores.Aplicacao.EventHandlers;

public class ProfessorCriadoEventHandler : INotificationHandler<ProfessorCriadoEvento>
{
    private readonly ILogger<ProfessorCriadoEventHandler> _logger;

    public ProfessorCriadoEventHandler(ILogger<ProfessorCriadoEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(ProfessorCriadoEvento notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Professor criado: {ProfessorId} - {Nome} (CPF: {Cpf}) na escola {EscolaId}",
            notification.ProfessorId,
            notification.Nome,
            notification.Cpf,
            notification.EscolaId);

        // Aqui poderiam ser implementadas outras ações como:
        // - Envio de email de boas-vindas
        // - Criação de usuário no sistema de autenticação
        // - Notificação para administradores da escola
        // - Integração com sistemas externos

        return Task.CompletedTask;
    }
}