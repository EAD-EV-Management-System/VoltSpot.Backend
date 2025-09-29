using Application.DTOs.Response.Users;
using MediatR;

namespace Application.UseCases.Users.Queries
{
    public class GetUserByIdQuery : IRequest<UserResponseDto?>
    {
        public string Id { get; set; } = string.Empty;
    }
}
