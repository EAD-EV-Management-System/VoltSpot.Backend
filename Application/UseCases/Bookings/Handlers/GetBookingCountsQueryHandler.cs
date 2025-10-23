using Application.UseCases.Bookings.Queries;
using AutoMapper;
using MediatR;
using VoltSpot.Application.DTOs.Response.Bookings;
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
            // Fetch counts in parallel for performance
            var pendingTask = _bookingRepository.GetPendingCountAsync(request.EvOwnerNic);
            var approvedTask = _bookingRepository.GetApprovedCountAsync(request.EvOwnerNic);
            var upcomingTask = _bookingRepository.GetUpcomingCountAsync(request.EvOwnerNic);

            await Task.WhenAll(pendingTask, approvedTask, upcomingTask);

            return new BookingCountsDto
            {
                PendingCount = pendingTask.Result,
                ApprovedCount = approvedTask.Result,
                UpcomingCount = upcomingTask.Result
            };
    }
}
}
