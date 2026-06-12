using Orders.Api.Interfaces;
using Shared.Requests;
using System.Net;

namespace Orders.Api.Services;

internal sealed class StocksClient(HttpClient httpClient) : IStocksClient
{
    public async Task<bool> ReserveStocksAsync(
        IEnumerable<ReservationItem> reservationItems,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "api/stocks/reservations",
            reservationItems,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Conflict)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return false;
    }
}
