using System;

namespace EventSource.Framework
{
    public interface IEventStore
    {
        bool Write<T>(T aggregate);
        T Get<T>(Guid id);

        TEventType AddEvent<TEventType>(Guid id, IVersionedEvent<Guid> eventItem) where TEventType : EventContainer;
    }
}