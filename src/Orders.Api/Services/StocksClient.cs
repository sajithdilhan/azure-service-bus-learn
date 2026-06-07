using System.Net;
using System.Net.Http.Json;
using Orders.Api.Interfaces;
using Shared.Requests;

namespace Orders.Api.Services;

public sealed class StocksClient(HttpClient httpClient) : IStocksClient
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
