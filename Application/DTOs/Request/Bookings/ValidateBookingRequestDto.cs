namespace Application.DTOs.Request.Bookings
{
    public class ValidateBookingRequestDto
    {
        public string ChargingStationId { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public int Duration { get; set; } = 2; // Default 2 hours
        public string? EvOwnerNic { get; set; }
    }
}
