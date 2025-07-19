using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando uma conta de usuário é bloqueada por tentativas de login falhadas.
/// </summary>
public record UserAccountLockedEvent(
    Guid UserId,
    string Email,
    DateTime LockedUntil
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}