using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando um usuário é ativado.
/// </summary>
public record UserActivatedEvent(
    Guid UserId,
    string Email
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}