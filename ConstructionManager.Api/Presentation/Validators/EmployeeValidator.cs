using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.NationalCode)
                .NotEmpty().WithMessage("National code is required")
                .Length(10).WithMessage("National code must be 10 digits")
                .Matches("^[0-9]+$").WithMessage("National code must contain only digits");

            RuleFor(x => x.EmployeeCode)
                .NotEmpty().WithMessage("Employee code is required")
                .MaximumLength(50).WithMessage("Employee code cannot exceed 50 characters");

            RuleFor(x => x.Phone1)
                .NotEmpty().WithMessage("Phone1 is required")
                .Length(11).WithMessage("Phone1 must be 11 digits")
                .Matches("^[0-9]+$").WithMessage("Phone1 must contain only digits");

            RuleFor(x => x.Phone2)
             .Length(11).WithMessage("Phone2 must be 11 digits")
             .Matches("^[0-9]+$").WithMessage("Phone2 must contain only digits")
             .When(x => !string.IsNullOrWhiteSpace(x.Phone2));

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required")
                .LessThan(DateTime.Now).WithMessage("Birth date must be in the past");

            RuleFor(x => x.Speciality)
                .MaximumLength(100).WithMessage("Speciality cannot exceed 100 characters");

            RuleFor(x => x.ExperienceYears)
                .GreaterThanOrEqualTo(0).When(x => x.ExperienceYears.HasValue)
                .WithMessage("Experience years cannot be negative");

            RuleFor(x => x.BaseSalary)
                .GreaterThanOrEqualTo(0).WithMessage("Base salary cannot be negative");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid employee status");
        }
    }
}
