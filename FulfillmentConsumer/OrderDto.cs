namespace FulfillmentConsumer
{
    public record OrderDto(int Id, OrderStatus Status, DateTimeOffset Date);
}
