using System.Text.Json.Serialization;

namespace OrderProvider.Orders
{
    public record OrderDto(int Id,
                           [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)]
                           OrderStatus Status,
                           DateTimeOffset Date);
}
