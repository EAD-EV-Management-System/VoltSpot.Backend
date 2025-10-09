using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Queries
{
    public class GetBookingCountsQuery : IRequest<BookingCountsDto>
    {
        public string EvOwnerNic { get; set; } = string.Empty; // optional filter if you want user-specific counts
    }
}
