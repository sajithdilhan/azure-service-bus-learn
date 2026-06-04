namespace Shared.Requests;

public sealed class CreateOrderLineRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
