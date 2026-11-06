using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class SupplierValidator : AbstractValidator<SupplierDto>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Supplier code is required")
                .MaximumLength(50).WithMessage("Supplier code cannot exceed 50 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required")
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters");

            RuleFor(x => x.Phone1)
                .NotEmpty().WithMessage("Phone1 is required")
                .Length(11).WithMessage("Phone1 must be 11 digits")
                .Matches("^[0-9]+$").WithMessage("Phone1 must contain only digits");

            RuleFor(x => x.Phone2)
                .Length(11).When(x => !string.IsNullOrWhiteSpace(x.Phone2))
                .WithMessage("Phone2 must be 11 digits")
                .Matches("^[0-9]+$").WithMessage("Phone2 must contain only digits");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
        }
    }
}
