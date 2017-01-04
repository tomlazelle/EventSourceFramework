namespace EventSource.Framework
{
    public interface IEventPublisher
    {
        void Publish<TMessage>(TMessage message);
    }
}