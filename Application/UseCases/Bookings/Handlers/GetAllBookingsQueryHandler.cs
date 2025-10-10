using Application.UseCases.Bookings.Queries;
using AutoMapper;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, List<BookingDetailDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetAllBookingsQueryHandler(
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDetailDto>> Handle(
            GetAllBookingsQuery request,
            CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetAllAsync(
                request.Page,
                request.PageSize,
                request.Status,
                request.EvOwnerNic,
                request.FromDate,
                request.ToDate);

            return _mapper.Map<List<BookingDetailDto>>(bookings);
        }
    }
}