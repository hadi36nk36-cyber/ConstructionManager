using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class EquipmentValidator : AbstractValidator<EquipmentDto>
    {
        public EquipmentValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Equipment code is required")
                .MaximumLength(50).WithMessage("Equipment code cannot exceed 50 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Equipment name is required")
                .MaximumLength(100).WithMessage("Equipment name cannot exceed 100 characters");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid equipment type");

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required")
                .MaximumLength(100).WithMessage("Serial number cannot exceed 100 characters");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid equipment status");

            RuleFor(x => x.LastServiceDate)
                .LessThanOrEqualTo(DateTime.Now).When(x => x.LastServiceDate.HasValue)
                .WithMessage("Last service date cannot be in the future");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
