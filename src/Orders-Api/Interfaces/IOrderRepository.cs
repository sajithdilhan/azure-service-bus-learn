using Shared.Entities;

namespace Orders.Api.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<Order>> GetOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(Order order);
}
