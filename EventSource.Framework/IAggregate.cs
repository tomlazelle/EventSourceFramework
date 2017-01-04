namespace EventSource.Framework
{
    public interface IAggregate<TId>
    {
        TId Id { get; }
    }
}