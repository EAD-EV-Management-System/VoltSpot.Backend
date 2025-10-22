using Application.UseCases.Bookings.Queries;
using AutoMapper;
using Domain.Enums;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetBookingsByEvOwnerQueryHandler : IRequestHandler<GetBookingsByEvOwnerQuery, List<BookingDetailDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        // Constructor: Inject dependencies
        public GetBookingsByEvOwnerQueryHandler(
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        // Handle method: Retrieves and converts bookings with optional filtering
        public async Task<List<BookingDetailDto>> Handle(
            GetBookingsByEvOwnerQuery request,
            CancellationToken cancellationToken)
        {
            // Get all bookings for this EV owner from database
            var bookings = await _bookingRepository.GetBookingsByEvOwnerAsync(request.EvOwnerNic);

            // Apply client-side filtering if parameters provided
            // Note: This is temporary until repository method supports filtering
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                bookings = bookings.Where(b =>
                    b.Id.ToLower().Contains(searchTerm) ||
                    b.ChargingStationId.ToLower().Contains(searchTerm)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<BookingStatus>(request.Status, true, out var statusEnum))
            {
                bookings = bookings.Where(b => b.Status == statusEnum).ToList();
            }

            // Convert list of Booking entities to list of DTOs
            return _mapper.Map<List<BookingDetailDto>>(bookings);
        }
    }
}