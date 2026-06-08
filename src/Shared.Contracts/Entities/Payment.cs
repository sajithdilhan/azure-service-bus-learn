using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("payments", Schema = "payments")]
public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentStatus Status { get; set; }
    public Guid TransactionId { get; set; }
    public string Reference { get; set; } = string.Empty;
}
