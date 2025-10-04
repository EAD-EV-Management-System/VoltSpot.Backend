using Application.DTOs.Response.Auth;
using MediatR;

namespace Application.UseCases.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
