using Domain.Common;
using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class EVOwner : BaseEntity
    {
        [BsonElement("nic")]
        public string NIC { get; set; } = string.Empty; // Using NIC as primary key

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("status")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public AccountStatus Status { get; set; } = AccountStatus.Active;
    }
}
