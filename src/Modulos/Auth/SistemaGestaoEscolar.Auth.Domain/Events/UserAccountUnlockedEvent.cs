using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando uma conta de usuário é desbloqueada.
/// </summary>
public record UserAccountUnlockedEvent(
    Guid UserId,
    string Email
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}