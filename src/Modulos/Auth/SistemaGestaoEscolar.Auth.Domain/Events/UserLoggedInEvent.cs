using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando um usuário faz login com sucesso.
/// </summary>
public record UserLoggedInEvent(
    Guid UserId,
    string Email,
    DateTime LoginAt
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}