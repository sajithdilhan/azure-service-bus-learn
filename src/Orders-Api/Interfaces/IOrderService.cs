using Shared.Entities;
using Shared.Requests;

namespace Orders.Api.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyCollection<Order>> GetOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
}
