
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VoltSpot.Application.UseCases;
using VoltSpot.Application.DTOs;

namespace VoltSpot.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly CreateBookingUseCase _createBookingUseCase;
        private readonly CancelBookingUseCase _cancelBookingUseCase;

        public BookingController(
            CreateBookingUseCase createBookingUseCase,
            CancelBookingUseCase cancelBookingUseCase)
        {
            _createBookingUseCase = createBookingUseCase;
            _cancelBookingUseCase = cancelBookingUseCase;
        }


        /// Creates a new booking

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request data");

            var result = await _createBookingUseCase.ExecuteAsync(request);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        //[HttpPut("update")]
        //public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingRequestDto request)
        //{
        //    if (request == null)
        //        return BadRequest("Invalid request data");

        //    var result = await _updateBookingUseCase.ExecuteAsync(request);

        //    if (result.Success)
        //        return Ok(result);
        //    else
        //        return BadRequest(result);
        //}


        /// Cancels an existing booking

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request data");

            var result = await _cancelBookingUseCase.ExecuteAsync(request);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}