using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Common.Models;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Queries
{
    public class GetPendingBookingCountQueryHandler : IRequestHandler<GetPendingBookingCountQuery, int>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingBookingCountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetPendingBookingCountQuery request, CancellationToken cancellationToken)
        {
            return await _context.Bookings.CountAsync(b => b.Status == "Pending", cancellationToken);
        }
    }
}
