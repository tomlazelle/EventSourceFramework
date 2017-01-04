using System;

namespace EventSource.Framework
{
    public class VersionedEvent<TSourceId> : IVersionedEvent<TSourceId>
    {
        public TSourceId SourceId { get; protected set; }
        public DateTime When { get; protected set; }
        public string EventType { get; protected set; }
        public int Version { get; set; }

        public VersionedEvent()
        {
            When = DateTime.Now;
        }
    }
}