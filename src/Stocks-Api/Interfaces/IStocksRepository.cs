using Shared.Entities;
using Shared.Requests;

namespace Stocks.Api.Interfaces;

public interface IStocksRepository
{
    Task<IReadOnlyCollection<Stock>> GetStocksAsync();
    Task<Stock?> GetStockByProductIdAsync(string productId);
    Task<Stock> CreateStockAsync(Stock stock);
    Task<Stock?> UpdateStockQuantityAsync(string productId, int quantityAvailable);
    Task<bool> ReserveStocksAsync(IEnumerable<ReservationItem> reservationItems);
}
