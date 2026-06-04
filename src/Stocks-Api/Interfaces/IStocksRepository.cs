using Shared.Entities;

namespace Stocks.Api.Interfaces;

public interface IStocksRepository
{
    Task<IReadOnlyCollection<Stock>> GetStocksAsync();
    Task<Stock?> GetStockByProductIdAsync(string productId);
    Task<Stock> CreateStockAsync(Stock stock);
    Task<Stock?> UpdateStockQuantityAsync(string productId, int quantityAvailable);
}
