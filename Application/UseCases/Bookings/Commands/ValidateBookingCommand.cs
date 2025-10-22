using Application.DTOs.Response.Bookings;
using MediatR;

namespace Application.UseCases.Bookings.Commands
{
    public class ValidateBookingCommand : IRequest<BookingValidationDto>
    {
        public string ChargingStationId { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public int Duration { get; set; } = 2;
        public string? EvOwnerNic { get; set; }
    }
}
