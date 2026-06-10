using Shared.Entities;
using Shared.Enums;

namespace Orders.Api.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<Order>> GetOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(Order order);
    Task<bool> UpdateOrderAsync(Order order);
    Task<bool> UpdateOrderStatusAsync(Guid id, OrderStatus status);
}
