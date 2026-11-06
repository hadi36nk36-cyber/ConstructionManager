using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class PurchaseInvoiceDetailValidator : AbstractValidator<PurchaseInvoiceDetailDto>
    {
        public PurchaseInvoiceDetailValidator()
        {
            RuleFor(x => x.PurchaseInvoiceId)
                .GreaterThan(0).WithMessage("Purchase invoice is required");

            RuleFor(x => x.MaterialName)
                .NotEmpty().WithMessage("Material name is required")
                .MaximumLength(100).WithMessage("Material name cannot exceed 100 characters");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("Unit is required")
                .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");

            RuleFor(x => x.TotalPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Total price cannot be negative");
        }
    }
}
