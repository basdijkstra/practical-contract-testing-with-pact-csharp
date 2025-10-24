using OrderProvider.PubSub;
using System.Collections.Concurrent;

namespace OrderProvider.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMessagePublisher publisher;

        // NOTE: for this demo this uses an in-memory store but in reality this would store to a database or something
        private readonly ConcurrentDictionary<int, OrderDto> orders = new();

        public OrderRepository(IMessagePublisher publisher)
        {
            this.publisher = publisher;
        }

        public Task<OrderDto> GetAsync(int id)
        {
            OrderDto order = this.orders[id];
            return Task.FromResult(order);
        }

        public async Task InsertAsync(OrderDto order)
        {
            this.orders[order.Id] = order;

            // notify subscribers of the new order
            await this.publisher.PublishAsync(new OrderCreatedEvent(order.Id));
        }

        public Task UpdateAsync(OrderDto order)
        {
            this.orders[order.Id] = order;
            return Task.CompletedTask;
        }
    }
}
