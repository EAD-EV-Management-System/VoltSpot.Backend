namespace Application.DTOs.Response.ChargingStations
{
    public class AvailableSlotsDto
    {
        public string StationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public List<int> AvailableSlots { get; set; } = new();
        public int AvailableCount { get; set; }
        public List<BookedSlotDto> BookedSlots { get; set; } = new();
        public List<int> MaintenanceSlots { get; set; } = new();
        public bool IsFullyBooked { get; set; }
    }

    public class BookedSlotDto
    {
        public int SlotNumber { get; set; }
        public string BookedBy { get; set; } = string.Empty;
        public DateTime BookedFrom { get; set; }
        public DateTime BookedUntil { get; set; }
        public string BookingId { get; set; } = string.Empty;
    }
}
