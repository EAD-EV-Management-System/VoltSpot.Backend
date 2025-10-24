using Application.UseCases.Bookings.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;
using MongoDB.Bson;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _chargingStationRepository;

        public UpdateBookingCommandHandler(
            IBookingRepository bookingRepository,
            IChargingStationRepository chargingStationRepository)
        {
            _bookingRepository = bookingRepository;
            _chargingStationRepository = chargingStationRepository;
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

            // Determine the duration to check
            var durationToCheck = request.NewDurationInMinutes ?? booking.DurationInMinutes;

            // Validate new duration if provided
            if (request.NewDurationInMinutes.HasValue)
            {
                if (request.NewDurationInMinutes.Value <= 0)
                {
                    throw new ArgumentException("Duration must be greater than 0 minutes");
                }

                if (request.NewDurationInMinutes.Value > 480) // Max 8 hours
                {
                    throw new ArgumentException("Duration cannot exceed 480 minutes (8 hours)");
                }
            }

            // Verify charging station exists
            var chargingStation = await _chargingStationRepository.GetByIdAsync(booking.ChargingStationId);
            if (chargingStation == null)
            {
                throw new KeyNotFoundException($"Charging station with ID {booking.ChargingStationId} not found");
            }

            // Check if new time slot is available with the duration, excluding the current booking
            var isAvailable = await _bookingRepository.IsSlotAvailableForDurationAsync(
                booking.ChargingStationId,
                booking.SlotNumber,
                request.NewReservationDateTime,
                durationToCheck,
                request.BookingId); // Exclude current booking from overlap check

            if (!isAvailable)
            {
                throw new InvalidOperationException(
                    $"Slot {booking.SlotNumber} is not available for the requested time period. " +
                    $"There is an overlapping booking between {request.NewReservationDateTime:yyyy-MM-dd HH:mm} " +
                    $"and {request.NewReservationDateTime.AddMinutes(durationToCheck):yyyy-MM-dd HH:mm}");
            }

            // Update the booking
            if (request.NewDurationInMinutes.HasValue)
            {
                booking.UpdateReservation(request.NewReservationDateTime, request.NewDurationInMinutes.Value);
            }
            else
            {
                booking.UpdateReservation(request.NewReservationDateTime);
            }

            await _bookingRepository.UpdateAsync(booking);
            return true;
        }
    }
}