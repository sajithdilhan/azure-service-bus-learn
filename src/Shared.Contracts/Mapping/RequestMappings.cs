using Shared.Entities;
using Shared.Enums;
using Shared.Requests;

namespace Shared.Mapping;

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
            PaymentMethod = Enum.TryParse(request.PaymentMethod, out PaymentMethods paymentMethod) ? paymentMethod : PaymentMethods.Unknown,
            Status = Enum.TryParse(request.PaymentStatus, out PaymentStatus paymentStatus) ? paymentStatus : PaymentStatus.Failed,
            TransactionId = request.TransactionId,
            Reference = request.Reference
        };
    }
}  
      
