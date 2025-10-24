using OrderProvider.Orders;

namespace OrderProvider.Order
{
    public record UpdateOrderRequest(int orderId, OrderStatus status);
}
