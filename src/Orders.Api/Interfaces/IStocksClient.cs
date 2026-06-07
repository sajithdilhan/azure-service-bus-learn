using Shared.Requests;

namespace Orders.Api.Interfaces;

public interface IStocksClient
{
    Task<bool> ReserveStocksAsync(IEnumerable<ReservationItem> reservationItems, CancellationToken cancellationToken = default);
}
