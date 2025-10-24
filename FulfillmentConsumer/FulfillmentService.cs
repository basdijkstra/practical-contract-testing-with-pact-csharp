namespace FulfillmentConsumer
{
    public class FulfillmentService : IFulfillmentService
    {
        private readonly IOrdersClient client;

        public FulfillmentService(IOrdersClient client)
        {
            this.client = client;
        }

        public async ValueTask FulfillOrderAsync(int orderId)
        {
            var order = await this.client.GetOrderAsync(orderId);
            await this.client.UpdateOrderAsync(order.Id, OrderStatus.Fulfilling);

            // fulfil and ship the order...

            await this.client.UpdateOrderAsync(order.Id, OrderStatus.Shipped);
        }
    }
}
