using Domain.Common;
using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class ChargingStation : BaseEntity
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("location")]
        public string Location { get; set; } = string.Empty;

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }

        [BsonElement("type")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public ChargingType Type { get; set; }

        [BsonElement("totalSlots")]
        public int TotalSlots { get; set; }

        [BsonElement("availableSlots")]
        public int AvailableSlots { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public StationStatus Status { get; set; } = StationStatus.Active;

        [BsonElement("operatingHours")]
        public OperatingHours OperatingHours { get; set; } = new();

        [BsonElement("assignedOperatorIds")]
        public List<string> AssignedOperatorIds { get; set; } = new();

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("amenities")]
        public List<string> Amenities { get; set; } = new();

        [BsonElement("pricePerHour")]
        public decimal PricePerHour { get; set; }

        public void UpdateAvailableSlots(int newAvailableSlots)
        {
            if (newAvailableSlots < 0 || newAvailableSlots > TotalSlots)
                throw new ArgumentException("Available slots must be between 0 and total slots");

            AvailableSlots = newAvailableSlots;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignOperator(string operatorId)
        {
            if (!AssignedOperatorIds.Contains(operatorId))
            {
                AssignedOperatorIds.Add(operatorId);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UnassignOperator(string operatorId)
        {
            if (AssignedOperatorIds.Remove(operatorId))
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Activate()
        {
            Status = StationStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = StationStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetMaintenance()
        {
            Status = StationStatus.Maintenance;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsOperational()
        {
            return Status == StationStatus.Active && AvailableSlots > 0;
        }

        public bool IsOperatingNow()
        {
            var now = DateTime.Now.TimeOfDay;
            return OperatingHours.IsOpen(now);
        }
    }

    public class OperatingHours
    {
        [BsonElement("openTime")]
        public TimeSpan OpenTime { get; set; } = new TimeSpan(0, 0, 0); // 00:00 (24/7 by default)

        [BsonElement("closeTime")]
        public TimeSpan CloseTime { get; set; } = new TimeSpan(23, 59, 59); // 23:59

        [BsonElement("is24Hours")]
        public bool Is24Hours { get; set; } = true;

        [BsonElement("closedDays")]
        public List<DayOfWeek> ClosedDays { get; set; } = new();

        public bool IsOpen(TimeSpan currentTime)
        {
            if (Is24Hours && !ClosedDays.Contains(DateTime.Now.DayOfWeek))
                return true;

            if (ClosedDays.Contains(DateTime.Now.DayOfWeek))
                return false;

            return currentTime >= OpenTime && currentTime <= CloseTime;
        }
    }
}