using Shared.Entities;
using Shared.Requests;
using Stocks.Api.Data;
using Stocks.Api.Interfaces;

namespace Stocks.Api.Repositories;

public sealed class StocksRepository(InMemoryStocksDatabase database) : IStocksRepository
{
    public Task<IReadOnlyCollection<Stock>> GetStocksAsync()
    {
        return Task.FromResult<IReadOnlyCollection<Stock>>(
            database.Stocks.Values.OrderBy(stock => stock.ProductName).ToList());
    }

    public Task<Stock?> GetStockByProductIdAsync(string productId)
    {
        database.Stocks.TryGetValue(productId, out var stock);
        return Task.FromResult(stock);
    }

    public Task<Stock> CreateStockAsync(Stock stock)
    {
        stock.CreatedAt = DateTime.UtcNow;
        stock.UpdatedAt = stock.CreatedAt;
        stock.LastRestockedAt = stock.CreatedAt;
        database.Stocks[stock.ProductId] = stock;

        return Task.FromResult(stock);
    }

    public Task<Stock?> UpdateStockQuantityAsync(string productId, int quantityAvailable)
    {
        if (!database.Stocks.TryGetValue(productId, out var stock))
        {
            return Task.FromResult<Stock?>(null);
        }

        stock.QuantityAvailable = quantityAvailable;
        stock.UpdatedAt = DateTime.UtcNow;
        stock.LastRestockedAt = stock.UpdatedAt;

        return Task.FromResult<Stock?>(stock);
    }

    public Task<bool> ReserveStocksAsync(IEnumerable<ReservationItem> reservationItems)
    {
        var requestedItems = reservationItems
            .GroupBy(item => item.ProductId, StringComparer.OrdinalIgnoreCase)
            .Select(group => new ReservationItem
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();

        lock (database.SyncRoot)
        {
            foreach (var item in requestedItems)
            {
                if (!database.Stocks.TryGetValue(item.ProductId, out var stock))
                {
                    return Task.FromResult(false);
                }

                if (stock.QuantityAvailable < item.Quantity)
                {
                    return Task.FromResult(false);
                }
            }

            foreach (var item in requestedItems)
            {
                var stock = database.Stocks[item.ProductId];
                stock.QuantityAvailable -= item.Quantity;
                stock.QuantityReserved += item.Quantity;
                stock.UpdatedAt = DateTime.UtcNow;
            }
        }

        return Task.FromResult(true);
    }
}
