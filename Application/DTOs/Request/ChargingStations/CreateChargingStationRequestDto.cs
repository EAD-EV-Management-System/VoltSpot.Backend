using Domain.Enums;

namespace Application.DTOs.Request.ChargingStations
{
    public class CreateChargingStationRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ChargingType Type { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public decimal PricePerHour { get; set; }
        public OperatingHoursDto OperatingHours { get; set; } = new();
        public List<string> AssignedOperatorIds { get; set; } = new();
    }

    public class OperatingHoursDto
    {
        public TimeSpan OpenTime { get; set; } = new TimeSpan(0, 0, 0);
        public TimeSpan CloseTime { get; set; } = new TimeSpan(23, 59, 59);
        public bool Is24Hours { get; set; } = true;
        public List<DayOfWeek> ClosedDays { get; set; } = new();
    }
}