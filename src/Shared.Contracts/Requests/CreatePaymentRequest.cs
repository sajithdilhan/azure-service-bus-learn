namespace Shared.Requests;

public sealed class CreatePaymentRequest
{
    public Guid OrderId { get; set; }
    public Guid TransactionId { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; } 
    public string Reference { get; set; } = string.Empty;
}
