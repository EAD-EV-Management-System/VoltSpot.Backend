using MediatR;
using VoltSpot.Application.DTOs;

namespace Application.UseCases.Bookings.Queries
{
    public class GetAllBookingsQuery : IRequest<List<BookingDetailDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Status { get; set; }
        public string? EvOwnerNic { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}