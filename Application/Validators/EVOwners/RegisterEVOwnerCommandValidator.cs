using Application.UseCases.EVOwners.Commands;
using FluentValidation;

namespace Application.Validators.EVOwners
{
    public class RegisterEVOwnerCommandValidator : AbstractValidator<RegisterEVOwnerCommand>
    {
        public RegisterEVOwnerCommandValidator()
        {
            RuleFor(x => x.NIC)
                .NotEmpty().WithMessage("NIC is required")
                .Matches(@"^[0-9]{9}[vVxX]$|^[0-9]{12}$")
                .WithMessage("NIC must be in valid Sri Lankan format (9 digits followed by V/X or 12 digits)");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be valid");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^[0-9]{10}$").WithMessage("Phone number must be 10 digits");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.Password).WithMessage("Passwords do not match");
        }
    }
}
