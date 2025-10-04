using Application.DTOs.Response.Auth;
using MediatR;

namespace Application.UseCases.Auth.Queries
{
    public class GetCurrentUserQuery : IRequest<UserInfoResponseDto>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
