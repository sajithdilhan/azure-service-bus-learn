using Shared.Enums;

namespace Shared.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentStatus Status { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}
