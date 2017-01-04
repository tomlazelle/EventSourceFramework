using System;

namespace EventSource.Framework
{
    public interface IEvent<out TSourceId>
    {
        TSourceId SourceId { get; }
        DateTime When { get; }
        string EventType { get; }
    }
}