using Payments.Api.Data;
using Payments.Api.Interfaces;
using Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Payments.Api.Repositories;

public class PaymentsRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    public async Task<bool> CreatePaymentAsync(Payment payment)
    {
        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();
        return true;
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
