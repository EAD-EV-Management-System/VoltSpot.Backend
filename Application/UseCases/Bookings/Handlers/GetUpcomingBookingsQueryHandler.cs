using AutoMapper;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;
using Application.UseCases.Bookings.Queries;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetUpcomingBookingsQueryHandler : IRequestHandler<GetUpcomingBookingsQuery, List<BookingDetailDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetUpcomingBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDetailDto>> Handle(GetUpcomingBookingsQuery request, CancellationToken cancellationToken)
        {
            // ✅ Use the correct repository method
            var bookings = await _bookingRepository.GetUpcomingBookingsAsync(request.EvOwnerNic);
            return _mapper.Map<List<BookingDetailDto>>(bookings);
        }
    }
}
