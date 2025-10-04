using MediatR;

namespace Application.UseCases.Auth.Commands
{
    public class LogoutCommand : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
