using Microsoft.AspNetCore.Mvc;
using Shared.Requests;
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
        var result = await stocksService.GetStocksAsync();
        if (!result.IsSuccess)
        {
            return StatusCode(result.Error?.Code ?? StatusCodes.Status404NotFound, result.Error);
        }

        return Ok(result);
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStockByProductId(string productId)
    {
        var result = await stocksService.GetStockByProductIdAsync(productId);
        if (!result.IsSuccess)
        {
            return StatusCode(result.Error?.Code ?? StatusCodes.Status404NotFound, result.Error);
        }

        return Ok(result);
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

        var result = await stocksService.CreateStockAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetStockByProductId), new { productId = result.Value?.ProductId }, result.Value);
    }

    [HttpPut("{productId}/quantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStockQuantity(string productId, UpdateStockRequest request)
    {
        var result = await stocksService.UpdateStockQuantityAsync(productId, request);
        if (!result.IsSuccess)
        {
            return StatusCode(result.Error?.Code ?? StatusCodes.Status404NotFound, result.Error);
        }

        return Ok(result);
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

        var result = await stocksService.ReserveStocksAsync(items);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error?.Code ?? StatusCodes.Status409Conflict, result.Error);
        }

        return Ok(result.Value);
    }
}
