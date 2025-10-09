using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Queries
{
    public class GetPendingBookingCountQuery : IRequest<int> { }
}
