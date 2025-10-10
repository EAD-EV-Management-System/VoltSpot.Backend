namespace Application.DTOs.Response.Dashboard
{
    public class StationUtilizationDto
    {
        public string StationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public int OccupiedSlots => TotalSlots - AvailableSlots;
        public double UtilizationPercentage => TotalSlots > 0 ? (double)OccupiedSlots / TotalSlots * 100 : 0;
        public int TotalBookings { get; set; }
        public decimal Revenue { get; set; }
    }
}