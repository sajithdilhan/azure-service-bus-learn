using Shared.Entities;

namespace Payments.Api.Interfaces;

public interface IPaymentRepository
{
    Task<bool> CreatePaymentAsync(Payment payment);
    Task<IReadOnlyCollection<Payment>> GetAllPaymentAsync();
    Task<Payment?> GetPaymentByIdAsync(Guid paymentId);
}
