using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Queries
{
    public class GetBookingByIdQuery : IRequest<BookingDetailDto>
    {
        public string BookingId { get; set; } = string.Empty;
    }
}