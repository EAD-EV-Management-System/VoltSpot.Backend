using Application.UseCases.Bookings.Commands;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _chargingStationRepository;

        // Constructor: Inject repositories
        public CompleteBookingCommandHandler(
            IBookingRepository bookingRepository,
            IChargingStationRepository chargingStationRepository)
        {
            _bookingRepository = bookingRepository;
            _chargingStationRepository = chargingStationRepository;
        }

        // Handle method: Completes the booking
        public async Task<bool> Handle(
            CompleteBookingCommand request,
            CancellationToken cancellationToken)
        {
            // Find the booking
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            //  Check if booking exists
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");
            }

            //  Check if booking is Confirmed (only Confirmed can be completed)
            if (booking.Status != BookingStatus.Confirmed)
            {
                throw new InvalidOperationException($"Cannot complete booking. Current status is {booking.Status}. Only Confirmed bookings can be completed.");
            }

            // Verify charging station exists
            var chargingStation = await _chargingStationRepository.GetByIdAsync(booking.ChargingStationId);
            if (chargingStation == null)
            {
                throw new KeyNotFoundException($"Charging station with ID {booking.ChargingStationId} not found");
            }

            //  Complete the booking
            booking.Complete();

            // Save to database
            await _bookingRepository.UpdateAsync(booking);

            // Note: Slot availability is now calculated dynamically based on time.
            // We no longer increment AvailableSlots as it doesn't represent time-based availability.

            return true;
        }
    }
}