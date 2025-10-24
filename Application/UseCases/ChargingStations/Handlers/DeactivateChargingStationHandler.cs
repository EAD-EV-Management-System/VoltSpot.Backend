using Application.UseCases.ChargingStations.Commands;
using Domain.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using VoltSpot.Domain.Interfaces;

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
            var allBookingsForStation = await _bookingRepository.GetBookingsByStationAsync(request.StationId);
            
            var activeBookings = allBookingsForStation
                .Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                .ToList();

            if (activeBookings.Any())
            {
                var bookingCount = activeBookings.Count;
                var bookingIds = string.Join(", ", activeBookings.Select(b => b.Id).Take(3));
                throw new InvalidOperationException(
                    $"Cannot deactivate station with {bookingCount} active booking(s). " +
                    $"Active booking IDs: {bookingIds}{(bookingCount > 3 ? "..." : "")}. " +
                    "Please ensure all active bookings are completed or cancelled before deactivating the station.");
            }

            // Deactivate the station
            station.Deactivate();
            
            // Update the station in the database
            var updatedStation = await _chargingStationRepository.UpdateAsync(station);
            
            return updatedStation != null;
        }
    }
}