using Application.DTOs.Response.Auth;
using MediatR;

namespace Application.UseCases.Auth.Commands
{
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
