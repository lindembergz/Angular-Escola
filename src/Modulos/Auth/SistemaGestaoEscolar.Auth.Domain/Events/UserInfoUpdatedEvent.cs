using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando as informações básicas de um usuário são atualizadas.
/// </summary>
public record UserInfoUpdatedEvent(
    Guid UserId,
    string Email,
    string OldFullName,
    string NewFullName
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}