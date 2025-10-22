using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Queries
{
    public class GetBookingsByEvOwnerQuery : IRequest<List<BookingDetailDto>>
    {
        public string EvOwnerNic { get; set; } = string.Empty;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
    }
}
