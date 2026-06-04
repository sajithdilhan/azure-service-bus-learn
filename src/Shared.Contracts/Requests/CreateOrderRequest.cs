namespace Shared.Requests;

public sealed class CreateOrderRequest
{
    public List<CreateOrderLineRequest> OrderLines { get; set; } = [];
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
}
