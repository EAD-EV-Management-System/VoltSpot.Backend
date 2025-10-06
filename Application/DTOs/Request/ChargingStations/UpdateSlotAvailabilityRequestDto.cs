namespace Application.DTOs.Request.ChargingStations
{
    public class UpdateSlotAvailabilityRequestDto
    {
        public string StationId { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
    }
}