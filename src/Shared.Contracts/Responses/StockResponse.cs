namespace Shared.Responses;

public sealed class StockResponse
{
    public Guid Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }
    public DateTime LastRestockedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
