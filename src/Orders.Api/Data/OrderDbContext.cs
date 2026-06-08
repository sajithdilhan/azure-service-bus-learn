using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Orders.Api.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders", "orders");

            entity.HasKey(order => order.Id);

            entity.Property(order => order.Id)
                .HasColumnName("id");

            entity.Property(order => order.OrderNumber)
                .HasColumnName("order_number")
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(order => order.CustomerId)
                .HasColumnName("customer_id")
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(order => order.CustomerName)
                .HasColumnName("customer_name")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(order => order.CustomerPhone)
                .HasColumnName("customer_phone")
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(order => order.CustomerEmail)
                .HasColumnName("customer_email")
                .HasMaxLength(254)
                .IsRequired();

            entity.Property(order => order.ShippingAddress)
                .HasColumnName("shipping_address")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(order => order.Status)
                .HasColumnName("status")
                .HasMaxLength(24)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(order => order.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(order => order.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Ignore(order => order.TotalAmount);

            entity.HasMany(order => order.OrderLines)
                .WithOne()
                .HasForeignKey("order_id")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(order => order.OrderNumber)
                .IsUnique();

            entity.HasIndex(order => order.CustomerId);
            entity.HasIndex(order => order.CreatedAt);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.ToTable("order_lines", "orders");

            entity.HasKey(orderLine => orderLine.Id);

            entity.Property(orderLine => orderLine.Id)
                .HasColumnName("id");

            entity.Property<Guid>("order_id")
                .HasColumnName("order_id");

            entity.Property(orderLine => orderLine.ProductId)
                .HasColumnName("product_id")
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(orderLine => orderLine.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(orderLine => orderLine.Quantity)
                .HasColumnName("quantity");

            entity.Property(orderLine => orderLine.Price)
                .HasColumnName("price")
                .HasPrecision(12, 2);

            entity.Property(orderLine => orderLine.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(orderLine => orderLine.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasIndex("order_id");
            entity.HasIndex(orderLine => orderLine.ProductId);
        });
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderLine> OrderLines { get; set; } = null!;
}
