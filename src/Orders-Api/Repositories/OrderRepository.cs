using Orders.Api.Data;
using Orders.Api.Interfaces;
using Shared.Entities;

namespace Orders.Api.Repositories;

public sealed class OrderRepository(InMemoryOrdersDatabase database) : IOrderRepository
{
    public Task<IReadOnlyCollection<Order>> GetOrdersAsync()
    {
        return Task.FromResult<IReadOnlyCollection<Order>>(
            database.Orders.Values.OrderByDescending(order => order.CreatedAt).ToList());
    }

    public Task<Order?> GetOrderByIdAsync(Guid id)
    {
        database.Orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<Order> CreateOrderAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = order.CreatedAt;
        database.Orders[order.Id] = order;

        return Task.FromResult(order);
    }
}
