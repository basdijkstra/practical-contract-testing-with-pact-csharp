namespace FulfillmentConsumer
{
    public interface IFulfillmentService
    {
        ValueTask FulfillOrderAsync(int orderId);
    }
}
