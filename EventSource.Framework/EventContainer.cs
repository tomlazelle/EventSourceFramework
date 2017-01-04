using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSource.Framework
{
    public abstract class EventContainer
    {
        public void Init(Guid id)
        {
            Id = id;
        }
        
        public void AddEvent(IVersionedEvent<Guid> eventItem)
        {
            var list = new List<IVersionedEvent<Guid>>();

            if (Events != null)
            {
                list.AddRange(Events);
            }

            var version = list.Any() ? list.Count + 1 : 1;

            eventItem.Version = version;

            list.Add(eventItem);

            Events = list.ToArray();
        }

        public Guid Id { get; private set; }
        public IVersionedEvent<Guid>[] Events { get; private set; }
    }
}