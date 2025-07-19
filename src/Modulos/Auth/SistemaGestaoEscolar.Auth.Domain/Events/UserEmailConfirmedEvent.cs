using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando um usuário confirma seu email.
/// </summary>
public record UserEmailConfirmedEvent(
    Guid UserId,
    string Email
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}