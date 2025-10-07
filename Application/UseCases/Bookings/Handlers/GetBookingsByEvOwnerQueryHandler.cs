
using Application.UseCases.Bookings.Queries;
using AutoMapper;
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

        // Handle method: Retrieves and converts bookings
        public async Task<List<BookingDetailDto>> Handle(
            GetBookingsByEvOwnerQuery request,
            CancellationToken cancellationToken)
        {
            // Get all bookings for this EV owner from database
            var bookings = await _bookingRepository.GetBookingsByEvOwnerAsync(request.EvOwnerNic);

            // Convert list of Booking entities to list of DTOs
            return _mapper.Map<List<BookingDetailDto>>(bookings);
        }
    }
}