using Azure.Messaging.ServiceBus;
using Orders.Api.Interfaces;
using Shared.Enums;
using Shared.MessagingContracts;
using System.Text.Json;

namespace Orders.Api.Services;

internal class PaymentConsumerService(
    ILogger<PaymentConsumerService> logger,
    ServiceBusClient serviceBusClient,
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = configuration["ServiceBus:Queues:Payments"] ?? "payments-queue";
        await using var paymentProcessor = serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        paymentProcessor.ProcessMessageAsync += async args =>
        {
            var body = args.Message.Body.ToString();
            logger.LogInformation("Received message: {Body}", body);

            var createPaymentMessage = JsonSerializer.Deserialize<CreatePaymentMessage>(body);

            if (createPaymentMessage is null)
            {
                logger.LogWarning("Received null message.");
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            if (createPaymentMessage.Status == PaymentStatus.Confirmed)
            {
                var orderRepository = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOrderRepository>();

                await orderRepository.UpdateOrderStatusAsync(createPaymentMessage.OrderId, OrderStatus.Confirmed);
                logger.LogInformation("Order {OrderId} status updated to Confirmed.", createPaymentMessage.OrderId);
            }

            await args.CompleteMessageAsync(args.Message);
        };

        paymentProcessor.ProcessErrorAsync += args =>
        {
            logger.LogError(args.Exception, "Error handling message.");
            return Task.CompletedTask;
        };

        await paymentProcessor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("PaymentConsumerService is stopping.");
            await paymentProcessor.StopProcessingAsync();
        }
    }
}
