namespace Application.DTOs.Request.ChargingStations
{
    public class DeactivateChargingStationRequestDto
    {
        public string? Reason { get; set; }
        public string? DeactivatedBy { get; set; }
        public DateTime? EstimatedReactivationDate { get; set; }
    }
}