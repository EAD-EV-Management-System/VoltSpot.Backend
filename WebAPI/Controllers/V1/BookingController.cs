using Application.UseCases.Bookings.Commands;
using Application.UseCases.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VoltSpot.Application.DTOs;
using WebAPI.Controllers.Base;
using Application.DTOs.Request.Bookings;

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

        // ✅ NEW: Validate booking before creation
        [HttpPost("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateBooking([FromBody] ValidateBookingRequestDto request)
        {
            var command = new ValidateBookingCommand
            {
                ChargingStationId = request.ChargingStationId,
                SlotNumber = request.SlotNumber,
                ReservationDateTime = request.ReservationDateTime,
                Duration = request.Duration,
                EvOwnerNic = request.EvOwnerNic
            };

            var result = await _mediator.Send(command);
            
            if (result.CanBook)
            {
                return Success(result, "Booking is valid and can be created");
            }
            else
            {
                return Success(result, "Booking validation completed with issues");
            }
        }

        // ✅ NEW: Get all bookings for admin management
        [HttpGet]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> GetAllBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? status = null,
            [FromQuery] string? evOwnerNic = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetAllBookingsQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                EvOwnerNic = evOwnerNic,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _mediator.Send(query);
            return Success(result);
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

        // ✅ NEW: Get bookings by user ID (mentioned in documentation)
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByUserId(string userId)
        {
            // For now, assuming userId is the same as EvOwnerNic
            // In a more complex system, you might need to map userId to EvOwnerNic
            var query = new GetBookingsByEvOwnerQuery
            {
                EvOwnerNic = userId
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

        [Authorize]
        [HttpGet("counts")]
        public async Task<IActionResult> GetBookingCounts()
        {
            var evOwnerNic = User.Claims.FirstOrDefault(c => c.Type == "nic")?.Value;
            var result = await _mediator.Send(new GetBookingCountsQuery { EvOwnerNic = evOwnerNic });
            return Success(result);
        }

        [Authorize]
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingBookings()
        {
            var evOwnerNic = User.Claims.FirstOrDefault(c => c.Type == "nic")?.Value;
            var result = await _mediator.Send(new GetUpcomingBookingsQuery { EvOwnerNic = evOwnerNic });
            return Success(result);
        }

        [Authorize]
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedBookings()
        {
            var evOwnerNic = User.Claims.FirstOrDefault(c => c.Type == "nic")?.Value;
            var result = await _mediator.Send(new GetCompletedBookingsQuery { EvOwnerNic = evOwnerNic });
            return Success(result);
        }

        [Authorize]
        [HttpGet("cancelled")]
        public async Task<IActionResult> GetCancelledBookings()
        {
            var evOwnerNic = User.Claims.FirstOrDefault(c => c.Type == "nic")?.Value;
            var result = await _mediator.Send(new GetCancelledBookingsQuery { EvOwnerNic = evOwnerNic });
            return Success(result);
        }
    }
}