using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class PaymentValidator : AbstractValidator<PaymentDto>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Payment code is required")
                .MaximumLength(50).WithMessage("Payment code cannot exceed 50 characters");

            RuleFor(x => x.TrackingCode)
                .NotEmpty().WithMessage("Tracking code is required")
                .MaximumLength(100).WithMessage("Tracking code cannot exceed 100 characters");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.PaymentDate)
                .NotEmpty().WithMessage("Payment date is required");

            RuleFor(x => x.PaymentType)
                .IsInEnum().WithMessage("Invalid payment type");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
