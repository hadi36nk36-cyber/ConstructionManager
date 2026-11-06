using ConstructionManager.Api.Application.DTOs;
using FluentValidation;

namespace ConstructionManager.Api.Presentation.Validators
{
    public class ProjectAssignmentValidator : AbstractValidator<ProjectAssignmentDto>
    {
        public ProjectAssignmentValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Project is required");

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee is required");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .MaximumLength(50).WithMessage("Role cannot exceed 50 characters");

            RuleFor(x => x.JoinDate)
                .NotEmpty().WithMessage("Join date is required");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.JoinDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("End date cannot be before join date");
        }
    }
}
