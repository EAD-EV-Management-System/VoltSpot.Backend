using Application.UseCases.Bookings.Queries;
using AutoMapper;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetCompletedBookingsQueryHandler : IRequestHandler<GetCompletedBookingsQuery, List<BookingDetailDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetCompletedBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDetailDto>> Handle(GetCompletedBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetCompletedBookingsAsync(request.EvOwnerNic);
            return _mapper.Map<List<BookingDetailDto>>(bookings);
        }
    }
}
