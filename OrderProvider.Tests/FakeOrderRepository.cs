using OrderProvider.Orders;
using System.Collections.Concurrent;

namespace OrderProvider.Tests
{
    public class FakeOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<int, OrderDto> orders = new();

        public Task<OrderDto> GetAsync(int id)
        {
            OrderDto order = this.orders[id];
            return Task.FromResult(order);
        }

        public Task InsertAsync(OrderDto order)
        {
            this.orders[order.Id] = order;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(OrderDto order)
        {
            this.orders[order.Id] = order;
            return Task.CompletedTask;
        }
    }
}
