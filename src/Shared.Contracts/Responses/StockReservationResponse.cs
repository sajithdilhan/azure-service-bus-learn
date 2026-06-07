namespace Shared.Responses;

public sealed class StockReservationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
