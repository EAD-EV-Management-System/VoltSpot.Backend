
using MediatR;

namespace Application.UseCases.Bookings.Commands
{
    public class CompleteBookingCommand : IRequest<bool>
    {
        public string BookingId { get; set; } = string.Empty;
    }
}