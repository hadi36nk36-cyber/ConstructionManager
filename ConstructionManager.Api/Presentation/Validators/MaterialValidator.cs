using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class MaterialValidator : AbstractValidator<MaterialDto>
    {
        public MaterialValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Material code is required")
                .MaximumLength(50).WithMessage("Material code cannot exceed 50 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Material name is required")
                .MaximumLength(100).WithMessage("Material name cannot exceed 100 characters");

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

            RuleFor(x => x.PurchaseDate)
                .NotEmpty().WithMessage("Purchase date is required");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.PurchaseDate)
                .When(x => x.ExpiryDate.HasValue)
                .WithMessage("Expiry date must be after purchase date");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
