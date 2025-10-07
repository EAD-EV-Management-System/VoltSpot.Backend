using Application.UseCases.Bookings.Queries;
using AutoMapper;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDetailDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        // Constructor: Inject repository and mapper
        public GetBookingByIdQueryHandler(
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        // Handle method: Does the actual work
        public async Task<BookingDetailDto> Handle(
            GetBookingByIdQuery request,
            CancellationToken cancellationToken)
        {
            //  Find booking in database
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            //  If not found, throw error
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");
            }

            //  Convert booking entity to DTO and return
            return _mapper.Map<BookingDetailDto>(booking);
        }
    }
}