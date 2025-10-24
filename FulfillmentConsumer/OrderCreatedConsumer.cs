namespace FulfillmentConsumer
{
    public class OrderCreatedConsumer
    {
        private readonly IFulfillmentService fulfillment;

        public OrderCreatedConsumer(IFulfillmentService fulfillment)
        {
            this.fulfillment = fulfillment;
        }

        public async ValueTask OnMessageAsync(OrderCreatedEvent message)
        {
            await this.fulfillment.FulfillOrderAsync(message.id);
        }
    }
}
