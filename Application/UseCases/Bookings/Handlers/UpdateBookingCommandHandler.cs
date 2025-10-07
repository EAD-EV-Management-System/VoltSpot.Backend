using Application.UseCases.Bookings.Commands;
using AutoMapper;
using Domain.Entities;
using MediatR;
using MongoDB.Bson;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        public UpdateBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
        {
            // Find the booking
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");
            }

            // Check if booking can be modified (12-hour rule)
            if (!booking.CanBeModified())
            {
                throw new InvalidOperationException("Booking cannot be modified. Must be at least 12 hours before reservation time");
            }

            // Check if new slot is available
            var isAvailable = await _bookingRepository.IsSlotAvailableAsync(
                booking.ChargingStationId,
                booking.SlotNumber,
                request.NewReservationDateTime);

            if (!isAvailable)
            {
                throw new InvalidOperationException("The selected time slot is not available");
            }

            // Update the booking
            booking.UpdateReservation(request.NewReservationDateTime);

            await _bookingRepository.UpdateAsync(booking);
            return true;
        }
    }
}