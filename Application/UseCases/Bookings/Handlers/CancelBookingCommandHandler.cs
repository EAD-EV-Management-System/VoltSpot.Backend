using Application.UseCases.Bookings.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _chargingStationRepository;

        public CancelBookingCommandHandler(
            IBookingRepository bookingRepository,
            IChargingStationRepository chargingStationRepository)
        {
            _bookingRepository = bookingRepository;
            _chargingStationRepository = chargingStationRepository;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");
            }

            // Check if booking can be cancelled (12 hour rule)
            if (!booking.CanBeModified())
            {
                throw new InvalidOperationException("Booking cannot be cancelled. Must be at least 12 hours before reservation time");
            }

            // Verify charging station exists
            var chargingStation = await _chargingStationRepository.GetByIdAsync(booking.ChargingStationId);
            if (chargingStation == null)
            {
                throw new KeyNotFoundException($"Charging station with ID {booking.ChargingStationId} not found");
            }

            // Use domain method to cancel booking
            booking.Cancel(request.CancellationReason ?? "No reason provided");
            await _bookingRepository.UpdateAsync(booking);

            // Note: Slot availability is now calculated dynamically based on time.
            // We no longer increment AvailableSlots as it doesn't represent time-based availability.

            return true;
        }
    } 
}
