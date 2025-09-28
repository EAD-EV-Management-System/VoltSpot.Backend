using Domain.Common;
using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("role")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public UserRole Role { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        [BsonElement("lastLoginAt")]
        public DateTime? LastLoginAt { get; set; }

        [BsonElement("assignedStationIds")]
        public List<string> AssignedStationIds { get; set; } = new List<string>();

        [BsonElement("refreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("refreshTokenExpiry")]
        public DateTime? RefreshTokenExpiry { get; set; }

        public bool CanAccessStation(string stationId)
        {
            if (Role == UserRole.Backoffice)
                return true; // Backoffice can access all stations

            return AssignedStationIds.Contains(stationId);
        }

        public void AssignStation(string stationId)
        {
            if (!AssignedStationIds.Contains(stationId))
            {
                AssignedStationIds.Add(stationId);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UnassignStation(string stationId)
        {
            if (AssignedStationIds.Remove(stationId))
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiry = expiry;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiry = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
