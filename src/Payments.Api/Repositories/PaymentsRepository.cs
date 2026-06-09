using Payments.Api.Data;
using Payments.Api.Interfaces;
using Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Payments.Api.Repositories;

public class PaymentsRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        payment.CreatedAt = DateTime.UtcNow;
        payment.UpdatedAt = payment.CreatedAt;
        payment.PaymentDate = payment.CreatedAt;

        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();

        return payment;
    }
    public async Task<IReadOnlyCollection<Payment>> GetAllPaymentAsync()
    {
        return await dbContext.Payments.ToListAsync();
    }
    public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
    {
        return await dbContext.Payments.FindAsync(paymentId);
    }
}
