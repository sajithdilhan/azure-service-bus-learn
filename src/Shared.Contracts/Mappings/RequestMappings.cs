using Shared.Entities;
using Shared.Enums;
using Shared.Requests;
using Shared.Responses;

namespace Shared.Mappings;

public static class RequestMappings
{
    public static Order ToEntity(this CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            ShippingAddress = request.ShippingAddress,
            Status = OrderStatus.Pending,
            OrderLines = request.OrderLines.Select(line => line.ToEntity()).ToList()
        };
        return order;
    }

    public static Order ToEntity(this OrderResponse response)
    {
        var order = new Order
        {
            CustomerId = response.CustomerId,
            CustomerName = response.CustomerName,
            CustomerPhone = response.CustomerPhone,
            CustomerEmail = response.CustomerEmail,
            ShippingAddress = response.ShippingAddress,
            Status = OrderStatus.Pending,
            OrderLines = response.OrderLines.Select(line => line.ToEntity()).ToList()
        };
        return order;
    }

    public static OrderLine ToEntity(this CreateOrderLineRequest request)
    {
        return new OrderLine
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            ProductName = request.ProductName,
            Price = request.Price
        };
    }

    public static OrderLine ToEntity(this OrderLineResponse response)
    {
        return new OrderLine
        {
            ProductId = response.ProductId,
            Quantity = response.Quantity,
            ProductName = response.ProductName,
            Price = response.Price
        };
    }

    public static Stock ToEntity(this CreateStockRequest request)
    {
        return new Stock
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            QuantityAvailable = request.QuantityAvailable,
            QuantityReserved = 0
        };
    }

    public static Payment ToEntity(this CreatePaymentRequest request)
    {
        return new Payment
        {
            OrderId = request.OrderId,
            Amount = request.TotalAmount,
            PaymentMethod = Enum.Parse<PaymentMethods>(request.PaymentMethod),
            Status = Enum.TryParse(request.PaymentStatus, out PaymentStatus paymentStatus) ? paymentStatus : PaymentStatus.Failed,
            TransactionId = request.TransactionId,
            Reference = request.Reference
        };
    }
}

