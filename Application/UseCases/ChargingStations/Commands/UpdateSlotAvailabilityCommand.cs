using MediatR;

namespace Application.UseCases.ChargingStations.Commands
{
    public class UpdateSlotAvailabilityCommand : IRequest<bool>
    {
        public string StationId { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
    }
}