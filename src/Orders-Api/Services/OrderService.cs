using Orders.Api.Interfaces;
using Shared.Entities;
using Shared.Enums;
using Shared.Requests;

namespace Orders.Api.Services;

public sealed class OrderService(IOrderRepository orderRepository) : IOrderService
{
    public Task<IReadOnlyCollection<Order>> GetOrdersAsync()
    {
        return orderRepository.GetOrdersAsync();
    }

    public Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return orderRepository.GetOrderByIdAsync(id);
    }

    public Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
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

        return orderRepository.CreateOrderAsync(order);
    }
}
