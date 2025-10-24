using Application.UseCases.ChargingStations.Commands;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    /// <summary>
    /// Handler for updating charging station slot configuration.
    /// Note: This should only be used to update TotalSlots (physical capacity).
    /// AvailableSlots are calculated dynamically based on time and bookings.
    /// </summary>
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

            // Only update total slots (physical capacity)
            // AvailableSlots should be calculated dynamically based on time and bookings
            if (request.TotalSlots > 0)
            {
                station.UpdateTotalSlots(request.TotalSlots);
            }
            
            // Deprecated: Manually updating AvailableSlots is incorrect for time-based slot availability
            // This is kept for backward compatibility but should not be used
            // Slot availability should be calculated using GET /api/chargingstations/{id}/available-slots
            if (request.AvailableSlots >= 0 && request.AvailableSlots != station.AvailableSlots)
            {
                // Log a warning that this is deprecated behavior
                System.Diagnostics.Debug.WriteLine(
                    $"WARNING: Manually updating AvailableSlots is deprecated. " +
                    $"Use GET /api/chargingstations/{{id}}/available-slots to check time-based availability.");
                
                station.UpdateAvailableSlots(request.AvailableSlots);
            }

            await _repository.UpdateAsync(station);

            return true;
        }
    }
}