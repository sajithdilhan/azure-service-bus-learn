using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Interfaces;
using Shared.Entities;

namespace Orders.Api.Repositories;

public sealed class OrderRepository(OrderDbContext database) : IOrderRepository
{
    public async Task<IReadOnlyCollection<Order>> GetOrdersAsync()
    {
        return await database.Orders
            .AsNoTracking()
            .Include(order => order.OrderLines)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await database.Orders
            .AsNoTracking()
            .Include(order => order.OrderLines)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = order.CreatedAt;
        database.Orders.Add(order);
        await database.SaveChangesAsync();

        return order;
    }

    public async Task UpdateOrderAsync(Order order)
    {
        if (!await database.Orders.AnyAsync(o => o.Id == order.Id))
        {
            throw new KeyNotFoundException($"Order with ID {order.Id} not found.");
        }
        order.UpdatedAt = DateTime.UtcNow;
        database.Orders.Update(order);
        await database.SaveChangesAsync();
    }
}
