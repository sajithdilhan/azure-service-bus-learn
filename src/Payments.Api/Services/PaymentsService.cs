using Azure.Messaging.ServiceBus;
using FluentValidation;
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
    IPaymentRepository paymentRepository,
    IValidator<CreatePaymentRequest> validator) : IPaymentService
{
    public async Task<Result<PaymentResponse>> ProcessPaymentAsync(CreatePaymentRequest request)
    {
        logger.LogInformation("Processing payment for order {OrderId}", request.OrderId);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
            logger.LogWarning("Invalid payment request for order {OrderId}. Errors: {ErrorMessages}", request.OrderId, string.Join(", ", errorMessages));
            return Result<PaymentResponse>.Failure(new Error((int)HttpStatusCode.BadRequest, string.Join(", ", errorMessages)));
        }

        var payment = request.ToEntity();

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
