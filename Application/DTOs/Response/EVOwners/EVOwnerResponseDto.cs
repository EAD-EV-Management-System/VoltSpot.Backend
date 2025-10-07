using Domain.Enums;

namespace Application.DTOs.Response.EVOwners
{
    public class EVOwnerResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public AccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
