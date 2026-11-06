using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Customer code is required")
                .MaximumLength(50).WithMessage("Customer code cannot exceed 50 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters");

            RuleFor(x => x.CustomerType)
                .IsInEnum().WithMessage("Invalid customer type");

            RuleFor(x => x.Phone1)
                .NotEmpty().WithMessage("Phone1 is required")
                .Length(11).WithMessage("Phone1 must be 11 digits")
                .Matches("^[0-9]+$").WithMessage("Phone1 must contain only digits");

            RuleFor(x => x.Phone2)
                .Length(11).When(x => !string.IsNullOrWhiteSpace(x.Phone2))
                .WithMessage("Phone2 must be 11 digits")
                .Matches("^[0-9]+$").When(x => !string.IsNullOrWhiteSpace(x.Phone2))
                .WithMessage("Phone2 must contain only digits");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
        }
    }
}
