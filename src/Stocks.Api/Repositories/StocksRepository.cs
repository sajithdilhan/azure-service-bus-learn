using Shared.Entities;
using Shared.Requests;
using Stocks.Api.Data;
using Stocks.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Api.Repositories;

public sealed class StocksRepository(StocksDbContext database) : IStocksRepository
{
    public async Task<IReadOnlyCollection<Stock>> GetStocksAsync()
    {
        return await database.Stocks.OrderBy(stock => stock.ProductName).ToListAsync();
    }

    public async Task<Stock?> GetStockByProductIdAsync(string productId)
    {
        return await database.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
    }

    public async Task<Stock> CreateStockAsync(Stock stock)
    {
        stock.CreatedAt = DateTime.UtcNow;
        stock.UpdatedAt = stock.CreatedAt;
        stock.LastRestockedAt = stock.CreatedAt;
        database.Stocks.Add(stock);
        await database.SaveChangesAsync();

        return stock;
    }

    public async Task<Stock?> UpdateStockQuantityAsync(string productId, int quantityAvailable)
    {
        var stock = await database.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
        if (stock == null)
        {
            return null;
        }

        stock.QuantityAvailable = quantityAvailable;
        stock.UpdatedAt = DateTime.UtcNow;
        stock.LastRestockedAt = stock.UpdatedAt;

        await database.SaveChangesAsync();

        return stock;
    }

    public async Task<bool> ReserveStocksAsync(IEnumerable<ReservationItem> reservationItems)
    {
        var requestedItems = reservationItems
            .GroupBy(item => item.ProductId, StringComparer.OrdinalIgnoreCase)
            .Select(group => new ReservationItem
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();

        return await Task.FromResult(true);
    }
}
