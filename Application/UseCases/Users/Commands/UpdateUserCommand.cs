using Application.DTOs.Response.Users;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserResponseDto>
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public AccountStatus Status { get; set; }
        public List<string> AssignedStationIds { get; set; } = new();
        public string UpdatedBy { get; set; } = string.Empty; // Current user ID
    }
}
