using Application.UseCases.Bookings.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltSpot.Application.DTOs;
using WebAPI.Controllers.Base;

namespace WebAPI.Controllers.V1
{
    public class BookingController : BaseController
    {
        private readonly IMediator _mediator;

        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
        {
            var command = new CreateBookingCommand
            {
                EvOwnerNic = request.EvOwnerNic,
                ChargingStationId = request.ChargingStationId,
                SlotNumber = request.SlotNumber,
                ReservationDateTime = request.ReservationDateTime
            };

            var result = await _mediator.Send(command);
            return Success(result, "Booking created successfully");
        }

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequestDto request)
        {
            var command = new CancelBookingCommand
            {
                BookingId = request.BookingId,
                CancellationReason = request.CancellationReason
            };

            var result = await _mediator.Send(command);
            return Success("Booking cancelled successfully");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingRequestDto request)
        {
            var command = new UpdateBookingCommand
            {
                BookingId = request.BookingId,
                NewReservationDateTime = request.NewReservationDateTime
            };

            var result = await _mediator.Send(command);
            return Success("Booking updated successfully");
        }
    }
}