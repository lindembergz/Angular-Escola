using MediatR;

namespace SistemaGestaoEscolar.Shared.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OcorridoEm { get; }
}