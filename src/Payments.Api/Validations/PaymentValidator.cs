using FluentValidation;
using Shared.Enums;
using Shared.Requests;

namespace Payments.Api.Validations;

public class PaymentValidator : AbstractValidator<CreatePaymentRequest>
{
   public PaymentValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderId is required.");
        RuleFor(x => x.TransactionId).NotEmpty().WithMessage("TransactionId is required.");
        RuleFor(x => x.PaymentStatus).NotEmpty().WithMessage("PaymentStatus is required.");
        RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("PaymentMethod is required.");
        RuleFor(x => x.TotalAmount).GreaterThan(0).WithMessage("TotalAmount must be greater than zero.");
        RuleFor(x => x.Reference).NotEmpty().WithMessage("Reference is required.");

        RuleFor(x => x.PaymentStatus)
            .Must(status => Enum.TryParse<PaymentStatus>(status, true, out _))
            .WithMessage("Invalid PaymentStatus value.");

        RuleFor(x => x.PaymentMethod)
            .Must(method => Enum.TryParse<PaymentMethods>(method, true, out _))
            .WithMessage("Invalid PaymentMethod value.");
    }
}
