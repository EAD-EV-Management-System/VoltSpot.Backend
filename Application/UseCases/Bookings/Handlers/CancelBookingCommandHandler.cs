using Application.UseCases.Bookings.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Application.DTOs;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        public CancelBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found");
            }

            // Use domain method to cancel booking
            booking.Cancel(request.CancellationReason ?? "No reason provided");

            await _bookingRepository.UpdateAsync(booking);
            return true;
        }
    } 
}
