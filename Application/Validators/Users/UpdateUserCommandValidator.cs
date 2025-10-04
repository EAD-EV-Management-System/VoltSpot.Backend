using Application.UseCases.Users.Commands;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Users
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required")
                .Length(24).WithMessage("Invalid User ID format");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role specified");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status specified");

            RuleFor(x => x.AssignedStationIds)
                .Must(BeValidStationIds).WithMessage("Invalid station IDs provided")
                .When(x => x.Role == UserRole.StationOperator);
        }

        private bool BeValidStationIds(List<string> stationIds)
        {
            if (stationIds == null) return true;
            return stationIds.All(id => !string.IsNullOrWhiteSpace(id) && id.Length == 24);
        }
    }
}
