using Orders.Api.Data;
using Orders.Api.Interfaces;
using Shared.Entities;

namespace Orders.Api.Repositories;

public sealed class OrderRepository(InMemoryOrdersDatabase database) : IOrderRepository
{
    public async Task<IReadOnlyCollection<Order>> GetOrdersAsync()
    {
        return await Task.FromResult<IReadOnlyCollection<Order>>(
            database.Orders.Values.OrderByDescending(order => order.CreatedAt).ToList());
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        database.Orders.TryGetValue(id, out var order);
        return await Task.FromResult(order);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = order.CreatedAt;
        database.Orders[order.Id] = order;

        return await Task.FromResult(order);
    }
}
