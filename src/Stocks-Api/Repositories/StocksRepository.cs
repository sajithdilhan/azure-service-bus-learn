using Shared.Entities;
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
}
