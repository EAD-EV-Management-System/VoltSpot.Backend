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
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponseDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _chargingStationRepository;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(
            IBookingRepository bookingRepository,
            IChargingStationRepository chargingStationRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _chargingStationRepository = chargingStationRepository;
            _mapper = mapper;
        }

        public async Task<BookingResponseDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate duration
            if (request.DurationInMinutes <= 0)
            {
                throw new ArgumentException("Duration must be greater than 0 minutes");
            }

            if (request.DurationInMinutes > 480) // Max 8 hours
            {
                throw new ArgumentException("Duration cannot exceed 480 minutes (8 hours)");
            }

            // 2. Verify charging station exists
            var chargingStation = await _chargingStationRepository.GetByIdAsync(request.ChargingStationId);
            if (chargingStation == null)
            {
                throw new KeyNotFoundException($"Charging station with ID {request.ChargingStationId} not found");
            }

            // 3. Validate slot number is within station's capacity
            if (request.SlotNumber < 1 || request.SlotNumber > chargingStation.TotalSlots)
            {
                throw new ArgumentException($"Slot number must be between 1 and {chargingStation.TotalSlots}");
            }

            // 4. Validate reservation time is not in the past
            if (request.ReservationDateTime < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Reservation time cannot be in the past");
            }

            // 5. Check slot availability for the requested time period (Time-based validation)
            var isAvailable = await _bookingRepository.IsSlotAvailableForDurationAsync(
                request.ChargingStationId,
                request.SlotNumber,
                request.ReservationDateTime,
                request.DurationInMinutes);

            if (!isAvailable)
            {
                throw new InvalidOperationException(
                    $"Slot {request.SlotNumber} is not available for the requested time period. " +
                    $"There is an overlapping booking between {request.ReservationDateTime:yyyy-MM-dd HH:mm} " +
                    $"and {request.ReservationDateTime.AddMinutes(request.DurationInMinutes):yyyy-MM-dd HH:mm}");
            }

            // 6. Create the booking with assigned slot and duration
            var booking = new Booking
            {
                Id = ObjectId.GenerateNewId().ToString(),
                EvOwnerNic = request.EvOwnerNic,
                ChargingStationId = request.ChargingStationId,
                SlotNumber = request.SlotNumber,
                BookingDate = DateTime.UtcNow,
                ReservationDateTime = request.ReservationDateTime,
                DurationInMinutes = request.DurationInMinutes,
                Status = Domain.Enums.BookingStatus.Pending,
                QrCode = Guid.NewGuid().ToString(), // Generate unique QR code for scanning
                CreatedAt = DateTime.UtcNow
            };

            var createdBooking = await _bookingRepository.AddAsync(booking);

            // Note: Slot availability is now calculated dynamically based on time.
            // We no longer decrement AvailableSlots as it doesn't represent time-based availability.

            return _mapper.Map<BookingResponseDto>(createdBooking);
        }
    }
}
