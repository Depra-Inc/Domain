using System.Collections.Generic;

namespace Depra.Domain
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents;

        public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        protected virtual void AddDomainEvent(IDomainEvent newEvent) => _domainEvents.Add(newEvent);

        public virtual void ClearEvents() => _domainEvents.Clear();

        protected AggregateRoot() => _domainEvents = new List<IDomainEvent>();
    }
}