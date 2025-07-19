using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando todas as sessões de um usuário são invalidadas.
/// </summary>
public record UserAllSessionsInvalidatedEvent(
    Guid UserId,
    string Email
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}