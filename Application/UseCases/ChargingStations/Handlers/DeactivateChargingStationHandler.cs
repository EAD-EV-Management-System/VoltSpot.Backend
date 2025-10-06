using Application.UseCases.ChargingStations.Commands;
using Domain.Interfaces.Repositories;
using VoltSpot.Domain.Interfaces;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class DeactivateChargingStationHandler : IRequestHandler<DeactivateChargingStationCommand, bool>
    {
        private readonly IChargingStationRepository _chargingStationRepository;
        private readonly IBookingRepository _bookingRepository;

        public DeactivateChargingStationHandler(
            IChargingStationRepository chargingStationRepository,
            IBookingRepository bookingRepository)
        {
            _chargingStationRepository = chargingStationRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> Handle(DeactivateChargingStationCommand request, CancellationToken cancellationToken)
        {
            // Check if station exists
            var station = await _chargingStationRepository.GetByIdAsync(request.StationId);
            if (station == null)
                throw new KeyNotFoundException($"Charging station with ID {request.StationId} not found");

            // Check for active bookings - cannot deactivate if active bookings exist
            var hasActiveBookings = await _chargingStationRepository.HasActiveBookingsAsync(request.StationId);
            if (hasActiveBookings)
                throw new InvalidOperationException("Cannot deactivate station with active bookings");

            // Deactivate the station
            station.Deactivate();
            await _chargingStationRepository.UpdateAsync(station);

            return true;
        }
    }
}