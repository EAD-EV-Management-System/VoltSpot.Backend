
using Application.UseCases.Bookings.Commands;
using Domain.Enums;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        // Constructor: Inject repository
        public CompleteBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // Handle method: Completes the booking
        public async Task<bool> Handle(
            CompleteBookingCommand request,
            CancellationToken cancellationToken)
        {
            // Find the booking
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            //  Check if booking exists
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");
            }

            //  Check if booking is Confirmed (only Confirmed can be completed)
            if (booking.Status != BookingStatus.Confirmed)
            {
                throw new InvalidOperationException($"Cannot complete booking. Current status is {booking.Status}. Only Confirmed bookings can be completed.");
            }

            //  Complete the booking
            booking.Complete();

            // Save to database
            await _bookingRepository.UpdateAsync(booking);

            return true;
        }
    }
}