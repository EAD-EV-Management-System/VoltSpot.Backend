using Application.UseCases.Bookings.Queries;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetBookingCountsQueryHandler : IRequestHandler<GetBookingCountsQuery, BookingCountsDto>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingCountsQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingCountsDto> Handle(GetBookingCountsQuery request, CancellationToken cancellationToken)
        {
            // Assuming repository supports a method to get count by status
            var pending = await _bookingRepository.CountByStatusAsync("Pending", request.EvOwnerNic);
            var approved = await _bookingRepository.CountByStatusAsync("Approved", request.EvOwnerNic);
            var upcoming = await _bookingRepository.CountByStatusAsync("Upcoming", request.EvOwnerNic);

            return new BookingCountsDto
            {
                PendingCount = pending,
                ApprovedCount = approved,
                UpcomingCount = upcoming
            };
        }
    }
}
