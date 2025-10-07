using Application.UseCases.EVOwners.Commands;
using FluentValidation;

namespace Application.Validators.EVOwners
{
    public class LoginEVOwnerCommandValidator : AbstractValidator<LoginEVOwnerCommand>
    {
        public LoginEVOwnerCommandValidator()
        {
            RuleFor(x => x.NIC)
                .NotEmpty().WithMessage("NIC is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
