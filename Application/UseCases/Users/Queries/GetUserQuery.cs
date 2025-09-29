using Application.DTOs.Response.Users;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Users.Queries
{
    public class GetUsersQuery : IRequest<IEnumerable<UserResponseDto>>
    {
        public UserRole? Role { get; set; }
        public AccountStatus? Status { get; set; }
        public string? SearchTerm { get; set; }
    }
}
