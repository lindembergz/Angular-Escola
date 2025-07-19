using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando uma tentativa de login falha.
/// </summary>
public record UserLoginFailedEvent(
    Guid UserId,
    string Email,
    int FailedAttempts
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}