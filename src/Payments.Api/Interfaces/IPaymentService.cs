using Shared.Common;
using Shared.Requests;
using Shared.Responses;

namespace Payments.Api.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentResponse>> ProcessPaymentAsync(CreatePaymentRequest request);
}
