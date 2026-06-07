using Payments.Api.Interfaces;
using Shared.Requests;

namespace Payments.Api.Services;

public class PaymentsService(ILogger<PaymentsService> logger) : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(CreatePaymentRequest request)
    {
        // Simulate payment processing logic here
        logger.LogInformation("Processing payment...");
        await Task.Delay(1000); // Simulating some async work
        logger.LogInformation("Payment message published to service bus");
        return true;
    }
}
