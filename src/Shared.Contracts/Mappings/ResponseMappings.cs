using Shared.Entities;
using Shared.Responses;

namespace Shared.Mappings;

public static class ResponseMappings
{
    public static OrderResponse ToResponse(this Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderLines = order.OrderLines.Select(line => line.ToResponse()).ToList(),
            TotalAmount = order.TotalAmount,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            CustomerEmail = order.CustomerEmail,
            ShippingAddress = order.ShippingAddress,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    public static OrderLineResponse ToResponse(this OrderLine orderLine)
    {
        return new OrderLineResponse
        {
            Id = orderLine.Id,
            ProductId = orderLine.ProductId,
            ProductName = orderLine.ProductName,
            Quantity = orderLine.Quantity,
            Price = orderLine.Price,
            LineTotal = orderLine.Price * orderLine.Quantity
        };
    }

    public static StockResponse ToResponse(this Stock stock)
    {
        return new StockResponse
        {
            Id = stock.Id,
            ProductId = stock.ProductId,
            ProductName = stock.ProductName,
            QuantityAvailable = stock.QuantityAvailable,
            QuantityReserved = stock.QuantityReserved,
            LastRestockedAt = stock.LastRestockedAt,
            CreatedAt = stock.CreatedAt,
            UpdatedAt = stock.UpdatedAt
        };
    }

    public static PaymentResponse ToResponse(this Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentDate = payment.PaymentDate,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            Reference = payment.Reference,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        };
    }
}
