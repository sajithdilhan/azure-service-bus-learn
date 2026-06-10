using Microsoft.AspNetCore.Mvc;
using Orders.Api.Interfaces;
using Shared.Requests;

namespace Orders.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders()
    {
        var result = await orderService.GetOrdersAsync();
        if (!result.IsSuccess)
        {
            return NotFound();
        }
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await orderService.GetOrderByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound();
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        if (request.OrderLines.Count == 0)
        {
            return BadRequest("At least one order line is required.");
        }

        var order = await orderService.CreateOrderAsync(request);
        if (!order.IsSuccess)
        {
            return BadRequest(order.Error);
        }
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Value?.Id }, order.Value);
    }
}
