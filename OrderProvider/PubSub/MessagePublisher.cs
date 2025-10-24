namespace OrderProvider.PubSub
{
    public class MessagePublisher : IMessagePublisher
    {
        public ValueTask PublishAsync<T>(T message)
        {
            // NOTE: For this demo we don't do anything, but a real implementation would publish to Kafka/RabbitMQ/etc

            return ValueTask.CompletedTask;
        }
    }
}
