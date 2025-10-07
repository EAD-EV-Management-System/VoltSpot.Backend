using Application.UseCases.Bookings.Commands;
using Application.UseCases.Bookings.Queries;
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

        /*
         * GET endpoint to retrieve a single booking by ID
         * Route: GET /api/v1/booking/{bookingId}
         */
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(string bookingId)
        {
            var query = new GetBookingByIdQuery
            {
                BookingId = bookingId
            };

            var result = await _mediator.Send(query);
            return Success(result);
        }

        [HttpGet("evowner/{evOwnerNic}")]
        public async Task<IActionResult> GetBookingsByEvOwner(string evOwnerNic)
        {
            var query = new GetBookingsByEvOwnerQuery
            {
                EvOwnerNic = evOwnerNic
            };

            var result = await _mediator.Send(query);
            return Success(result);
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingRequestDto request)
        {
            var command = new ConfirmBookingCommand
            {
                BookingId = request.BookingId
            };

            var result = await _mediator.Send(command);
            return Success("Booking confirmed successfully");
        }

        [HttpPut("complete")]
        public async Task<IActionResult> CompleteBooking([FromBody] CompleteBookingRequestDto request)
        {
            var command = new CompleteBookingCommand
            {
                BookingId = request.BookingId
            };

            var result = await _mediator.Send(command);
            return Success("Booking completed successfully");
        }

    }
}