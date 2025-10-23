using MediatR;
using VoltSpot.Application.DTOs.Response.Bookings;

namespace Application.UseCases.Bookings.Queries
{
    public class GetBookingCountsQuery : IRequest<BookingCountsDto>
    {
        public string EvOwnerNic { get; set; } = string.Empty;
}
}
