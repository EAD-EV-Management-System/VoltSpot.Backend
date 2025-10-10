using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Queries
{
    public class GetCompletedBookingsQuery : IRequest<List<BookingDetailDto>>
    {
        public string EvOwnerNic { get; set; } = string.Empty;
    }
}
