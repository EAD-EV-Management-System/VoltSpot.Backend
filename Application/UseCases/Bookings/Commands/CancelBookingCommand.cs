using MediatR;

namespace Application.UseCases.Bookings.Commands
{
    public class CancelBookingCommand : IRequest<bool>
    {
        public string BookingId { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
    }
}
