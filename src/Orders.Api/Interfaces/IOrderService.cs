using Shared.Common;
using Shared.Entities;
using Shared.Requests;
using Shared.Responses;

namespace Orders.Api.Interfaces;

public interface IOrderService
{
    Task<Result<IReadOnlyCollection<OrderResponse>>> GetOrdersAsync();
    Task<Result<OrderResponse>> GetOrderByIdAsync(Guid id);
    Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest request);
    Task<Result<bool>> UpdateOrderAsync(Order order);
}
