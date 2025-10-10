namespace Application.DTOs.Response.Dashboard
{
    public class RecentBookingDto
    {
        public string Id { get; set; } = string.Empty;
        public string EvOwnerNic { get; set; } = string.Empty;
        public string EvOwnerName { get; set; } = string.Empty;
        public string ChargingStationId { get; set; } = string.Empty;
        public string ChargingStationName { get; set; } = string.Empty;
        public DateTime ReservationDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int SlotNumber { get; set; }
    }
}