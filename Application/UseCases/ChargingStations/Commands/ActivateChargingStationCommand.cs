using MediatR;

namespace Application.UseCases.ChargingStations.Commands
{
    public class ActivateChargingStationCommand : IRequest<bool>
    {
        public string StationId { get; set; } = string.Empty;
        public string? ActivatedBy { get; set; }
        public string? Notes { get; set; }
    }
}
