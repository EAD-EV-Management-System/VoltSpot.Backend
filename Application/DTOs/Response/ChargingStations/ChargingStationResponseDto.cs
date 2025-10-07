using Domain.Enums;

namespace Application.DTOs.Response.ChargingStations
{
    public class ChargingStationResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public decimal PricePerHour { get; set; }
        public OperatingHoursResponseDto OperatingHours { get; set; } = new();
        public List<string> AssignedOperatorIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsOperational { get; set; }
        public bool IsOperatingNow { get; set; }
    }

    public class OperatingHoursResponseDto
    {
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool Is24Hours { get; set; }
        public List<DayOfWeek> ClosedDays { get; set; } = new();
    }
}