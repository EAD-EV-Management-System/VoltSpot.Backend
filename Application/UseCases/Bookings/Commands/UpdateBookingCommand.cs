using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Commands
{
    public class UpdateBookingCommand : IRequest<bool>
    {
        public string BookingId { get; set; } = string.Empty;
        public DateTime NewReservationDateTime { get; set; }
        public int? NewDurationInMinutes { get; set; } // Optional - only update if provided
    }
}
