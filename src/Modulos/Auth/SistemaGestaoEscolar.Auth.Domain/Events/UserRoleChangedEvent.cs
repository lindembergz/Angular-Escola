using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Auth.Domain.Events;

/// <summary>
/// Evento disparado quando o papel de um usuário é alterado.
/// </summary>
public record UserRoleChangedEvent(
    Guid UserId,
    string Email,
    string OldRole,
    string NewRole
) : IDomainEvent
{
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}