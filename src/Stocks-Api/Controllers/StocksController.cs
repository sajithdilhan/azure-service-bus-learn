using Microsoft.AspNetCore.Mvc;
using Shared.Mapping;
using Shared.Requests;
using Shared.Responses;
using Stocks.Api.Interfaces;

namespace Stocks.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StocksController(IStocksService stocksService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStocks()
    {
        var stocks = await stocksService.GetStocksAsync();
        return Ok(stocks.Select(stock => stock.ToResponse()));
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStockByProductId(string productId)
    {
        var stock = await stocksService.GetStockByProductIdAsync(productId);
        if (stock == null)
        {
            return NotFound();
        }

        return Ok(stock.ToResponse());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStock(CreateStockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductId))
        {
            return BadRequest("Product id is required.");
        }

        var stock = await stocksService.CreateStockAsync(request);
        return CreatedAtAction(nameof(GetStockByProductId), new { productId = stock.ProductId }, stock.ToResponse());
    }

    [HttpPut("{productId}/quantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStockQuantity(string productId, UpdateStockRequest request)
    {
        var stock = await stocksService.UpdateStockQuantityAsync(productId, request);
        if (stock == null)
        {
            return NotFound();
        }

        return Ok(stock.ToResponse());
    }

    [HttpPost("reservations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reservations(IEnumerable<ReservationItem> reservationItems)
    {
        var items = reservationItems?.ToList();
        if (items == null || items.Count == 0)
        {
            return BadRequest("At least one reservation item is required.");
        }

        if (items.Any(item => string.IsNullOrWhiteSpace(item.ProductId) || item.Quantity <= 0))
        {
            return BadRequest("Each reservation item requires a product id and a quantity greater than zero.");
        }

        var success = await stocksService.ReserveStocksAsync(items);

        if (!success)
        {
            return Conflict(new StockReservationResponse
            {
                Success = false,
                Message = "One or more reservations could not be processed."
            });
        }

        return Ok(new StockReservationResponse
        {
            Success = true,
            Message = "Stocks reserved successfully."
        });
    }
}
