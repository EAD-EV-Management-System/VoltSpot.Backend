using Application.UseCases.EVOwners.Commands;
using FluentValidation;

namespace Application.Validators.EVOwners
{
    public class UpdateEVOwnerCommandValidator : AbstractValidator<UpdateEVOwnerCommand>
    {
        public UpdateEVOwnerCommandValidator()
        {
            RuleFor(x => x.NIC)
                .NotEmpty()
                .WithMessage("NIC is required")
                .Matches(@"^[0-9]{9}[vVxX]$")
                .WithMessage("NIC must be in format: 9 digits followed by 'V' or 'X'");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(50)
                .WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(50)
                .WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email must be a valid email address");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .Matches(@"^[0-9]{10}$")
                .WithMessage("Phone number must be 10 digits");
        }
    }
}