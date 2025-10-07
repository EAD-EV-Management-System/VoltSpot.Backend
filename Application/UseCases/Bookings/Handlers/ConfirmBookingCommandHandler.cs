
using Application.UseCases.Bookings.Commands;
using Domain.Enums;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        // Constructor: Inject repository
        public ConfirmBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // Handle method: Confirms the booking
        public async Task<bool> Handle(
            ConfirmBookingCommand request,
            CancellationToken cancellationToken)
        {
            //Find the booking
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            // Check if booking exists
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");
            }

            //  Check if booking is in Pending status
            if (booking.Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot confirm booking. Current status is {booking.Status}. Only Pending bookings can be confirmed.");
            }

            //  Confirm the booking (uses domain method)
            booking.Confirm();

            //  Save to database
            await _bookingRepository.UpdateAsync(booking);

            return true;
        }
    }
}