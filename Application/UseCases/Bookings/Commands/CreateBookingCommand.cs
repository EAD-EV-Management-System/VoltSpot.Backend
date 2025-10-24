using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Commands
{
    public class CreateBookingCommand : IRequest<BookingResponseDto>
    {
        public string EvOwnerNic { get; set; } = string.Empty;
        public string ChargingStationId { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public int DurationInMinutes { get; set; } = 120; // Default 2 hours (120 minutes)
    }
}
