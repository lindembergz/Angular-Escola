using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando um novo usuário é criado no sistema.
/// </summary>
public record UserCreatedEvent(
    Guid UserId,
    string Email,
    string Role
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}