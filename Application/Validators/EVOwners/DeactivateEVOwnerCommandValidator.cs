using Application.UseCases.EVOwners.Commands;
using FluentValidation;

namespace Application.Validators.EVOwners
{
    public class DeactivateEVOwnerCommandValidator : AbstractValidator<DeactivateEVOwnerCommand>
    {
        public DeactivateEVOwnerCommandValidator()
        {
            RuleFor(x => x.NIC)
                .NotEmpty()
                .WithMessage("NIC is required")
                .Matches(@"^[0-9]{9}[vVxX]$")
                .WithMessage("NIC must be in format: 9 digits followed by 'V' or 'X'");
        }
    }
}
