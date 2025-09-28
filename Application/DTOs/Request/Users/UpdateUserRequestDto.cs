using Domain.Enums;

namespace Application.DTOs.Request.Users
{
    public class UpdateUserRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public AccountStatus Status { get; set; }
        public List<string> AssignedStationIds { get; set; } = new();
    }
}
