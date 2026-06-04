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
        var orders = await orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
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
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }
}
