using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class PurchaseInvoiceValidator : AbstractValidator<PurchaseInvoiceDto>
    {
        public PurchaseInvoiceValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Purchase invoice code is required")
                .MaximumLength(50).WithMessage("Purchase invoice code cannot exceed 50 characters");

            RuleFor(x => x.TrackingCode)
                .NotEmpty().WithMessage("Tracking code is required")
                .MaximumLength(100).WithMessage("Tracking code cannot exceed 100 characters");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Project is required");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier is required");

            RuleFor(x => x.InvoiceDate)
                .NotEmpty().WithMessage("Invoice date is required");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid purchase invoice status");
        }
    }
}
