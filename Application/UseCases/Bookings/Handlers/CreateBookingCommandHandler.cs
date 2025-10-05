using Application.UseCases.Bookings.Commands;
using AutoMapper;
using Domain.Entities;
using MediatR;
using MongoDB.Bson;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponseDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<BookingResponseDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // Check slot availability
            var isAvailable = await _bookingRepository.IsSlotAvailableAsync(
                request.ChargingStationId,
                request.SlotNumber,
                request.ReservationDateTime);

            if (!isAvailable)
            {
                throw new InvalidOperationException("The selected slot is not available for the requested time");
            }

            var booking = new Booking
            {
                Id = ObjectId.GenerateNewId().ToString(),
                EvOwnerNic = request.EvOwnerNic,
                ChargingStationId = request.ChargingStationId,
                SlotNumber = request.SlotNumber,
                BookingDate = DateTime.UtcNow,
                ReservationDateTime = request.ReservationDateTime,
                Status = Domain.Enums.BookingStatus.Pending,
                QrCode = Guid.NewGuid().ToString(), // Generate unique QR code for scanning
                CreatedAt = DateTime.UtcNow
            };

            var createdBooking = await _bookingRepository.AddAsync(booking);
            return _mapper.Map<BookingResponseDto>(createdBooking);
        }
    }
}
