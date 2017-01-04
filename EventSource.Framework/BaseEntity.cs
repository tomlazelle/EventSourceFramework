using System;
using System.Collections.Generic;

namespace EventSource.Framework
{
    public class BaseEntity<TId> : IAggregate<TId>
    {
        protected readonly Dictionary<Type, Action<IVersionedEvent<TId>>> _handlers = new Dictionary<Type, Action<IVersionedEvent<TId>>>();

        protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : IEvent<TId>
        {
            _handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        protected BaseEntity(TId id)
        {
            Id = id;
        }

        public TId Id { get; }

        public int Version { get; private set; }

        protected void LoadEvents(IEnumerable<IVersionedEvent<TId>> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                _handlers[e.GetType()].Invoke(e);
                Version = e.Version;
            }
        }
    }
}