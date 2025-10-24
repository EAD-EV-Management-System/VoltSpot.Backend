using Application.UseCases.Bookings.Queries;
using AutoMapper;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetBookingsByOperatorQueryHandler : IRequestHandler<GetBookingsByOperatorQuery, List<BookingDetailDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetBookingsByOperatorQueryHandler(
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDetailDto>> Handle(
            GetBookingsByOperatorQuery request,
            CancellationToken cancellationToken)
        {
            // Get all bookings for charging stations assigned to this operator
            var bookings = await _bookingRepository.GetBookingsByOperatorAsync(request.OperatorId);

            // Convert list of Booking entities to list of DTOs
            return _mapper.Map<List<BookingDetailDto>>(bookings);
        }
    }
}
