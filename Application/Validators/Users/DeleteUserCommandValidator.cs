using Application.UseCases.Users.Commands;
using FluentValidation;

namespace Application.Validators.Users
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required")
                .Length(24).WithMessage("Invalid User ID format");
        }
    }
}
