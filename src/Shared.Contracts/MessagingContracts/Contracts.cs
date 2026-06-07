using Shared.Enums;

namespace Shared.MessagingContracts;

public record CreatePaymentMessage(Guid OrderId, decimal Amount, DateTime Timestamp, PaymentStatus Status);
