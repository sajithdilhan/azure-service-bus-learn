using Orders.Api.Exceptions;
using Orders.Api.Interfaces;
using Shared.Entities;
using Shared.Mapping;
using Shared.Requests;

namespace Orders.Api.Services;

public sealed class OrderService(IOrderRepository orderRepository, IStocksClient stocksClient, ILogger<OrderService> logger) : IOrderService
{
    public async Task<IReadOnlyCollection<Order>> GetOrdersAsync()
    {
        return await orderRepository.GetOrdersAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await orderRepository.GetOrderByIdAsync(id);
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
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
            throw new StockReservationFailedException();
        }

        return await orderRepository.CreateOrderAsync(request.ToEntity());
    }

    public async Task UpdateOrderAsync(Order order)
    {
        try
        {
            await orderRepository.UpdateOrderAsync(order);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the order.");
            throw;
        }

    }
}
