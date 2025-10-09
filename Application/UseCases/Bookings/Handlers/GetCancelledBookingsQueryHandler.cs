using AutoMapper;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetCancelledBookingsQueryHandler : IRequestHandler<GetCancelledBookingsQuery, List<BookingDetailDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetCancelledBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDetailDto>> Handle(GetCancelledBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetBookingsByStatusAsync("Cancelled", request.EvOwnerNic);
            return _mapper.Map<List<BookingDetailDto>>(bookings);
        }
    }
}
