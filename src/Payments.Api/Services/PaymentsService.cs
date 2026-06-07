using Azure.Messaging.ServiceBus;
using OpenTelemetry.Trace;
using Payments.Api.Interfaces;
using Shared.Enums;
using Shared.MessagingContracts;
using Shared.Requests;
using System.Text.Json;

namespace Payments.Api.Services;

public class PaymentsService(ILogger<PaymentsService> logger, ServiceBusClient client, IConfiguration configuration) : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(CreatePaymentRequest request)
    {
        logger.LogInformation("Processing payment for order {OrderId}", request.OrderId);

        if (request.PaymentStatus != nameof(PaymentStatus.Pending))
        {
            logger.LogWarning("Invalid payment status for order {OrderId}: {Status}", request.OrderId, request.PaymentStatus);
            return false;
        }

        var paymentMessage = new CreatePaymentMessage(request.OrderId, request.TotalAmount, DateTime.UtcNow, PaymentStatus.Confirmed);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(paymentMessage));

        var queueName = configuration["ServiceBus:Queues:Payments"] ?? "payments-queue";
        var sender = client.CreateSender(queueName);
        await sender.SendMessageAsync(message);
        logger.LogInformation("Payment message published to service bus queue {QueueName}", queueName);
        return true;
    }
}
