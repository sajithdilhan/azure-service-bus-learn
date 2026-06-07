namespace Shared.Requests;

public sealed class ReservationItem
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}   
