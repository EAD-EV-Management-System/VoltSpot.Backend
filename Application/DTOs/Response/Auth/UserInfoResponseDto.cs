using Domain.Enums;

namespace Application.DTOs.Response.Auth
{
    public class UserInfoResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public UserRole Role { get; set; }
        public AccountStatus Status { get; set; }
        public List<string> AssignedStationIds { get; set; } = new();
        public DateTime? LastLoginAt { get; set; }
    }
}
