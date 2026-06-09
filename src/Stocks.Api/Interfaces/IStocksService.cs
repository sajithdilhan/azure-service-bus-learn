using Shared.Common;
using Shared.Requests;
using Shared.Responses;

namespace Stocks.Api.Interfaces;

public interface IStocksService
{
    Task<Result<IReadOnlyCollection<StockResponse>>> GetStocksAsync();
    Task<Result<StockResponse>> GetStockByProductIdAsync(string productId);
    Task<Result<StockResponse>> CreateStockAsync(CreateStockRequest request);
    Task<Result<StockResponse>> UpdateStockQuantityAsync(string productId, UpdateStockRequest request);
    Task<Result<bool>> HasAvailableStockAsync(string productId, int quantity);
    Task<Result<StockReservationResponse>> ReserveStocksAsync(IEnumerable<ReservationItem> reservationItems);
}
