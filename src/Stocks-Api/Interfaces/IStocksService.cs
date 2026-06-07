using Shared.Entities;
using Shared.Requests;

namespace Stocks.Api.Interfaces;

public interface IStocksService
{
    Task<IReadOnlyCollection<Stock>> GetStocksAsync();
    Task<Stock?> GetStockByProductIdAsync(string productId);
    Task<Stock> CreateStockAsync(CreateStockRequest request);
    Task<Stock?> UpdateStockQuantityAsync(string productId, UpdateStockRequest request);
    Task<bool> HasAvailableStockAsync(string productId, int quantity);
    Task<bool> ReserveStocksAsync(IEnumerable<ReservationItem> reservationItems);
}
