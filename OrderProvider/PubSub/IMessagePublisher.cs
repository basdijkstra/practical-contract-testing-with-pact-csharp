namespace OrderProvider.PubSub
{
    public interface IMessagePublisher
    {
        ValueTask PublishAsync<T>(T message);
    }
}
