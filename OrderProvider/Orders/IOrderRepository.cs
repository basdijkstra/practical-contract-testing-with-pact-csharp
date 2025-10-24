namespace OrderProvider.Orders
{
    public interface IOrderRepository
    {
        Task<OrderDto> GetAsync(int id);

        Task InsertAsync(OrderDto order);

        Task UpdateAsync(OrderDto order);
    }
}
