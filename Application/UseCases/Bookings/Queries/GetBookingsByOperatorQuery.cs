using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Queries
{
    public class GetBookingsByOperatorQuery : IRequest<List<BookingDetailDto>>
    {
        public string OperatorId { get; set; } = string.Empty;
    }
}
