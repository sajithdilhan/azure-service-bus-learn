using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("orders", Schema = "orders")]
public sealed class Order : BaseEntity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public List<OrderLine> OrderLines { get; set; } = [];
    public decimal TotalAmount => OrderLines.Sum(ol => ol.Price * ol.Quantity);
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }

    public Order()
    {
        OrderNumber = GenerateOrderNumber();
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
}
