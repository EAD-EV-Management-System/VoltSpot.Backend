using MediatR;

namespace Application.UseCases.Users.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public string DeletedBy { get; set; } = string.Empty; // Current user ID
    }
}
