using Orders.Api.Interfaces;
using Shared.Common;
using Shared.Mappings;
using Shared.Requests;
using Shared.Responses;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace Orders.Api.Services;

internal sealed class OrderService(IOrderRepository orderRepository,
    IStocksClient stocksClient,
    ILogger<OrderService> logger,
    IConnectionMultiplexer connectionMultiplexer) : IOrderService
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> GetOrdersAsync()
    {
        var orders = await orderRepository.GetOrdersAsync();

        if (orders == null || !orders.Any())
        {
            logger.LogInformation("No orders found in the database.");
            return Result<IReadOnlyCollection<OrderResponse>>.Failure(new Error((int)HttpStatusCode.NotFound, "No orders found!"));
        }
        return Result<IReadOnlyCollection<OrderResponse>>.Success(orders.Select(order => order.ToResponse()).ToList());
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(Guid id)
    {

        var cacheKey = $"order:{id}";
        var db = connectionMultiplexer.GetDatabase();
        var cachedOrder = await db.StringGetAsync(cacheKey);

        if (cachedOrder.HasValue)
        {
            logger.LogInformation("Order found in cache with ID: {OrderId}", id);
            var orderResponse = JsonSerializer.Deserialize<OrderResponse>(cachedOrder.ToString());
            if (orderResponse == null)
            {
                logger.LogError("Failed to deserialize cached order with ID: {OrderId}", id);
                return Result<OrderResponse>.Failure(new Error((int)HttpStatusCode.InternalServerError, "Failed to retrieve order from cache!"));
            }
            return Result<OrderResponse>.Success(orderResponse);
        }

        var order = await orderRepository.GetOrderByIdAsync(id);

        if (order == null)
        {
            logger.LogInformation("Order not found with ID: {OrderId}", id);
            return Result<OrderResponse>.Failure(new Error((int)HttpStatusCode.NotFound, "Order not found!"));
        }

        db.StringSet(cacheKey, JsonSerializer.Serialize(order.ToResponse()), TimeSpan.FromMinutes(10));
        return Result<OrderResponse>.Success(order.ToResponse());
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest request)
    {
        var reservationItems = request.OrderLines.Select(line => new ReservationItem
        {
            ProductId = line.ProductId,
            Quantity = line.Quantity
        }).ToList();

        var stocksReserved = await stocksClient.ReserveStocksAsync(reservationItems);
        if (!stocksReserved)
        {
            logger.LogError("Failed to reserve stocks for order creation. Customer: {CustomerId}, OrderLines: {OrderLines}",
                request.CustomerId, string.Join(", ", request.OrderLines.Select(line => $"ProductId: {line.ProductId}, Quantity: {line.Quantity}")));
            return Result<OrderResponse>.Failure(new Error((int)HttpStatusCode.BadRequest, "Failed to reserve stocks for the order!"));
        }

        var order = await orderRepository.CreateOrderAsync(request.ToEntity());
        return Result<OrderResponse>.Success(order.ToResponse());
    }

    public async Task<Result<bool>> UpdateOrderAsync(Shared.Entities.Order order)
    {
        var result = await orderRepository.UpdateOrderAsync(order);
        if (!result)
        {
            logger.LogError("Failed to update order with ID: {OrderId}", order.Id);
            return Result<bool>.Failure(new Error((int)HttpStatusCode.InternalServerError, "Failed to update order!"));
        }
        return Result<bool>.Success(result);
    }
}
