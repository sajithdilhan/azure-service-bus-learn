using Orders.Api.Exceptions;
using Orders.Api.Interfaces;
using Shared.Entities;
using Shared.Enums;
using Shared.Requests;

namespace Orders.Api.Services;

public sealed class OrderService(IOrderRepository orderRepository, IStocksClient stocksClient) : IOrderService
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
            throw new StockReservationFailedException();
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            ShippingAddress = request.ShippingAddress,
            Status = OrderStatus.Pending,
            OrderLines = request.OrderLines.Select(line => new OrderLine
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                ProductName = line.ProductName,
                Price = line.Price
            }).ToList()
        };

        return await orderRepository.CreateOrderAsync(order);
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await orderRepository.UpdateOrderAsync(order);
    }
}
