using Application.DTOs.Response.Dashboard;
using MediatR;

namespace Application.UseCases.Dashboard.Queries
{
    public class GetRecentBookingsQuery : IRequest<List<RecentBookingDto>>
    {
        public int Count { get; set; } = 10;
    }
}