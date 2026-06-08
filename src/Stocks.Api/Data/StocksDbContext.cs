using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Stocks.Api.Data;

public class StocksDbContext : DbContext
{
    public StocksDbContext(DbContextOptions<StocksDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("stocks", "stocks");

            entity.HasKey(stock => stock.Id);

            entity.Property(stock => stock.Id)
                .HasColumnName("id");

            entity.Property(stock => stock.ProductId)
                .HasColumnName("product_id")
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(stock => stock.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(stock => stock.QuantityAvailable)
                .HasColumnName("quantity_available");

            entity.Property(stock => stock.QuantityReserved)
                .HasColumnName("quantity_reserved");

            entity.Property(stock => stock.LastRestockedAt)
                .HasColumnName("last_restocked_at");

            entity.Property(stock => stock.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(stock => stock.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasIndex(stock => stock.ProductId)
                .IsUnique();
        });
    }
    public DbSet<Stock> Stocks { get; set; } = null!;
}
