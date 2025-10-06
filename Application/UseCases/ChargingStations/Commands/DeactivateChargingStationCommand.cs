using MediatR;

namespace Application.UseCases.ChargingStations.Commands
{
    public class DeactivateChargingStationCommand : IRequest<bool>
    {
        public string StationId { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? DeactivatedBy { get; set; }
        public DateTime? EstimatedReactivationDate { get; set; }
    }
}