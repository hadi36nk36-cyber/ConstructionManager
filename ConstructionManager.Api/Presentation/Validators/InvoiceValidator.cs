using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class InvoiceValidator : AbstractValidator<InvoiceDto>
    {
        public InvoiceValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Invoice code is required")
                .MaximumLength(50).WithMessage("Invoice code cannot exceed 50 characters");

            RuleFor(x => x.TrackingCode)
                .NotEmpty().WithMessage("Tracking code is required")
                .MaximumLength(100).WithMessage("Tracking code cannot exceed 100 characters");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Project is required");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer is required");

            RuleFor(x => x.InvoiceDate)
                .NotEmpty().WithMessage("Invoice date is required");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid invoice status");
        }
    }
}
