using Application.UseCases.Bookings.Queries;
using MediatR;
using VoltSpot.Domain.Interfaces;
using Domain.Enums; // If you store BookingStatus as Enum

namespace Application.UseCases.Bookings.Handlers
{
    public class GetPendingBookingCountQueryHandler : IRequestHandler<GetPendingBookingCountQuery, int>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetPendingBookingCountQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<int> Handle(GetPendingBookingCountQuery request, CancellationToken cancellationToken)
        {
            // Fetch bookings of the specific EV owner
            var bookings = await _bookingRepository.GetBookingsByEvOwnerAsync(request.EvOwnerNic);

            // Count only pending ones
            return bookings.Count(b => b.Status == BookingStatus.Pending);
        }
    }
}
