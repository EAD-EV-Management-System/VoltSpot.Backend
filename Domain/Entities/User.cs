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
    }
}
