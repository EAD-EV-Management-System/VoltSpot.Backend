using Application.UseCases.ChargingStations.Commands;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class ActivateChargingStationHandler : IRequestHandler<ActivateChargingStationCommand, bool>
    {
        private readonly IChargingStationRepository _chargingStationRepository;

        public ActivateChargingStationHandler(IChargingStationRepository chargingStationRepository)
        {
            _chargingStationRepository = chargingStationRepository;
        }

        public async Task<bool> Handle(ActivateChargingStationCommand request, CancellationToken cancellationToken)
        {
            // Check if station exists
            var station = await _chargingStationRepository.GetByIdAsync(request.StationId);
            if (station == null)
                throw new KeyNotFoundException($"Charging station with ID {request.StationId} not found");

            // Activate the station
            station.Activate();
            
            // Update the station in the database
            var updatedStation = await _chargingStationRepository.UpdateAsync(station);
            
            return updatedStation != null;
        }
    }
}
