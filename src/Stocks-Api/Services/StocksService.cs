using Shared.Entities;
using Shared.Requests;
using Stocks.Api.Interfaces;

namespace Stocks.Api.Services;

public sealed class StocksService(IStocksRepository stocksRepository) : IStocksService
{
    public Task<IReadOnlyCollection<Stock>> GetStocksAsync()
    {
        return stocksRepository.GetStocksAsync();
    }

    public Task<Stock?> GetStockByProductIdAsync(string productId)
    {
        return stocksRepository.GetStockByProductIdAsync(productId);
    }

    public Task<Stock> CreateStockAsync(CreateStockRequest request)
    {
        var stock = new Stock
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            QuantityAvailable = request.QuantityAvailable
        };

        return stocksRepository.CreateStockAsync(stock);
    }

    public Task<Stock?> UpdateStockQuantityAsync(string productId, UpdateStockRequest request)
    {
        return stocksRepository.UpdateStockQuantityAsync(productId, request.QuantityAvailable);
    }

    public async Task<bool> HasAvailableStockAsync(string productId, int quantity)
    {
        var stock = await stocksRepository.GetStockByProductIdAsync(productId);
        return stock is not null && stock.QuantityAvailable - stock.QuantityReserved >= quantity;
    }
}
