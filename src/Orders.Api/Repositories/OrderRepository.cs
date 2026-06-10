using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Interfaces;
using Shared.Entities;
using Shared.Enums;

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

    public async Task<bool> UpdateOrderAsync(Order order)
    {
       var existingOrder = await database.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.Id == order.Id);
        if (existingOrder == null)
        {
            return false;
        }
        existingOrder.CustomerId = order.CustomerId;
        existingOrder.CustomerName = order.CustomerName;
        existingOrder.CustomerPhone = order.CustomerPhone;
        existingOrder.CustomerEmail = order.CustomerEmail;
        existingOrder.ShippingAddress = order.ShippingAddress;
        existingOrder.Status = order.Status;
        existingOrder.UpdatedAt = DateTime.UtcNow;
        // Update order lines
        database.OrderLines.RemoveRange(existingOrder.OrderLines);
        existingOrder.OrderLines = order.OrderLines;
        await database.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        var existingOrder = await database.Orders
             .FirstOrDefaultAsync(o => o.Id == orderId);
        if (existingOrder == null)
        {
            return false;
        }
        existingOrder.Status = status;
        existingOrder.UpdatedAt = DateTime.UtcNow;
        await database.SaveChangesAsync();
        return true;
    }
}
