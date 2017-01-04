namespace EventSource.Framework
{
    public interface IMessageHandler<in TMessage,out TResult> where TMessage : class
    {
        TResult Handle(TMessage message);
    }
}