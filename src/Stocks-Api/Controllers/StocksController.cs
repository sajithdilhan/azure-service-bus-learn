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
        var stocks = await stocksService.GetStocksAsync();
        return Ok(stocks);
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

        return Ok(stock);
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
        return CreatedAtAction(nameof(GetStockByProductId), new { productId = stock.ProductId }, stock);
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

        return Ok(stock);
    }
}
