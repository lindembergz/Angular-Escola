using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Shared.Domain.Entities;

public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() : base() { }

    protected AggregateRoot(Guid id) : base(id) { }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public bool HasDomainEvents => _domainEvents.Any();

    public IDomainEvent[] GetUncommittedEvents()
    {
        return _domainEvents.ToArray();
    }

    public void MarkEventsAsCommitted()
    {
        _domainEvents.Clear();
    }
}