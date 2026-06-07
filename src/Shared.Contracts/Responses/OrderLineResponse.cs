namespace Shared.Responses;

public sealed class OrderLineResponse
{
    public Guid Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal LineTotal { get; set; }
}
