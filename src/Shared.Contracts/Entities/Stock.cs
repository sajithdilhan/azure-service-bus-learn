using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Entities;

[Table("stocks", Schema = "stocks")]
public sealed class Stock : BaseEntity
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }
    public DateTime LastRestockedAt { get; set; } = DateTime.UtcNow;
}
