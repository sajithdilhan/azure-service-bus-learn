namespace Shared.Requests;

public sealed class CreateStockRequest
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
}
