using Application.UseCases.ChargingStations.Commands;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class UpdateSlotAvailabilityHandler : IRequestHandler<UpdateSlotAvailabilityCommand, bool>
    {
        private readonly IChargingStationRepository _repository;

        public UpdateSlotAvailabilityHandler(IChargingStationRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateSlotAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var station = await _repository.GetByIdAsync(request.StationId);
            if (station == null)
                throw new KeyNotFoundException($"Charging station with ID {request.StationId} not found");

            station.UpdateAvailableSlots(request.AvailableSlots);
            await _repository.UpdateAsync(station);

            return true;
        }
    }
}