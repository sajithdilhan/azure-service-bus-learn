using Shared.Requests;

namespace Payments.Api.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(CreatePaymentRequest request);
}
