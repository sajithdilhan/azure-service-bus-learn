using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Payments.Api.Data;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments", "payments");

            entity.HasKey(payment => payment.Id);

            entity.Property(payment => payment.Id)
                .HasColumnName("id");

            entity.Property(payment => payment.OrderId)
                .HasColumnName("order_id");

            entity.Property(payment => payment.Amount)
                .HasColumnName("amount")
                .HasPrecision(12, 2);

            entity.Property(payment => payment.PaymentMethod)
                .HasColumnName("payment_method")
                .HasMaxLength(24)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(payment => payment.PaymentDate)
                .HasColumnName("payment_date");

            entity.Property(payment => payment.Status)
                .HasColumnName("status")
                .HasMaxLength(24)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(payment => payment.TransactionId)
                .HasColumnName("transaction_id");

            entity.Property(payment => payment.Reference)
                .HasColumnName("reference")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(payment => payment.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(payment => payment.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasIndex(payment => payment.OrderId);
            entity.HasIndex(payment => payment.Status);
            entity.HasIndex(payment => payment.PaymentDate);
            entity.HasIndex(payment => payment.TransactionId)
                .IsUnique();
        });
    }

    public DbSet<Payment> Payments { get; set; } = null!;
}
