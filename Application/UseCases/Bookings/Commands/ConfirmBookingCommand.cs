using MediatR;

namespace Application.UseCases.Bookings.Commands
{
    public class ConfirmBookingCommand : IRequest<bool>
    {
        public string BookingId { get; set; } = string.Empty;
    }
}