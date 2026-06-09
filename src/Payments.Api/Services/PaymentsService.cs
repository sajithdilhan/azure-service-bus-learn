using Azure.Messaging.ServiceBus;
using Payments.Api.Interfaces;
using Shared.Common;
using Shared.Enums;
using Shared.Mappings;
using Shared.MessagingContracts;
using Shared.Requests;
using Shared.Responses;
using System.Net;
using System.Text.Json;

namespace Payments.Api.Services;

public class PaymentsService(
    ILogger<PaymentsService> logger,
    ServiceBusClient client,
    IConfiguration configuration,
    IPaymentRepository paymentRepository) : IPaymentService
{
    public async Task<Result<PaymentResponse>> ProcessPaymentAsync(CreatePaymentRequest request)
    {
        logger.LogInformation("Processing payment for order {OrderId}", request.OrderId);

        if (request.PaymentStatus != nameof(PaymentStatus.Confirmed))
        {
            logger.LogWarning("Invalid payment status for order {OrderId}: {Status}", request.OrderId, request.PaymentStatus);
            return Result<PaymentResponse>.Failure(new Error((int)HttpStatusCode.BadRequest, "Invalid payment status."));
        }

        var payment = request.ToEntity();
        if (payment.PaymentMethod == PaymentMethods.Unknown)
        {
            logger.LogWarning("Invalid payment method for order {OrderId}: {PaymentMethod}", request.OrderId, request.PaymentMethod);
            return Result<PaymentResponse>.Failure(new Error((int)HttpStatusCode.BadRequest, "Invalid payment method."));
        }

        payment = await paymentRepository.CreatePaymentAsync(payment);

        var paymentMessage = new CreatePaymentMessage(request.OrderId, request.TotalAmount, DateTime.UtcNow, PaymentStatus.Confirmed);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(paymentMessage));

        var queueName = configuration["ServiceBus:Queues:Payments"] ?? "payments-queue";
        var sender = client.CreateSender(queueName);
        await sender.SendMessageAsync(message);
        logger.LogInformation("Payment message published to service bus queue {QueueName}", queueName);

        return Result<PaymentResponse>.Success(payment.ToResponse());
    }
}
