using MediatR;

namespace Application.UseCases.ChargingStations.Commands
{
    public class DeactivateChargingStationCommand : IRequest<bool>
    {
        public string StationId { get; set; } = string.Empty;
    }
}