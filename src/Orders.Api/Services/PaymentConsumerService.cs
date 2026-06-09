using Azure.Messaging.ServiceBus;
using Orders.Api.Interfaces;
using Shared.Enums;
using Shared.Mappings;
using Shared.MessagingContracts;
using System.Text.Json;

namespace Orders.Api.Services;

public class PaymentConsumerService(
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

            if(createPaymentMessage is null)
            {
                logger.LogWarning("Received null message.");
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            if (createPaymentMessage.Status == PaymentStatus.Confirmed)
            {
                var orderService = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOrderService>();
                var order = await orderService.GetOrderByIdAsync(createPaymentMessage.OrderId);
                if (order is { IsSuccess: true , Value: not null })
                {
                    order.Value.Status = OrderStatus.Confirmed;
                    await orderService.UpdateOrderAsync(order.Value.ToEntity());
                    logger.LogInformation("Order {OrderId} status updated to Confirmed.", createPaymentMessage.OrderId);
                }
                else
                {
                    logger.LogWarning("Order {OrderId} not found.", createPaymentMessage.OrderId);
                }
            }

            await args.CompleteMessageAsync(args.Message);
        };

        paymentProcessor.ProcessErrorAsync += args =>
        {
            logger.LogError(args.Exception, "Error handling message.");
            return Task.CompletedTask;
        };

        await paymentProcessor.StartProcessingAsync(stoppingToken);
        logger.LogInformation("Payment consumer started for queue {QueueName}.", queueName);

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Payment consumer is stopping.");
        }
        finally
        {
            await paymentProcessor.StopProcessingAsync(CancellationToken.None);
        }
    }
}
