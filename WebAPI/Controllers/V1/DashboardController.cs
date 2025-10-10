using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Base;
using Application.UseCases.Dashboard.Queries;

namespace WebAPI.Controllers.V1
{
    [Authorize(Roles = "Backoffice")]
    public class DashboardController : BaseController
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get dashboard statistics for admin overview
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var query = new GetDashboardStatsQuery();
            var result = await _mediator.Send(query);
            return Success(result);
        }

        /// <summary>
        /// Get recent bookings for dashboard
        /// </summary>
        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings([FromQuery] int count = 10)
        {
            try
            {
                var query = new GetRecentBookingsQuery { Count = count };
                var result = await _mediator.Send(query);
                return Success(result);
            }
            catch (Exception ex)
            {
                // Return detailed error information for debugging
                return Error($"Error retrieving recent bookings: {ex.Message}. Inner: {ex.InnerException?.Message}", 500);
            }
        }

        /// <summary>
        /// Get station utilization data for dashboard
        /// </summary>
        [HttpGet("station-utilization")]
        public async Task<IActionResult> GetStationUtilization()
        {
            var query = new GetStationUtilizationQuery();
            var result = await _mediator.Send(query);
            return Success(result);
        }

        // ? TEST ENDPOINTS (Remove these after testing)
        /// <summary>
        /// Test endpoint for dashboard stats - any authenticated user
        /// </summary>
        [HttpGet("test-stats")]
        [Authorize]
        public async Task<IActionResult> GetDashboardStatsTest()
        {
            var query = new GetDashboardStatsQuery();
            var result = await _mediator.Send(query);
            return Success(result);
        }

        /// <summary>
        /// Test endpoint for recent bookings - any authenticated user with detailed error info
        /// </summary>
        [HttpGet("test-recent-bookings")]
        [Authorize]
        public async Task<IActionResult> GetRecentBookingsTest([FromQuery] int count = 10)
        {
            try
            {
                var query = new GetRecentBookingsQuery { Count = count };
                var result = await _mediator.Send(query);
                return Success(result, $"Retrieved {result.Count} recent bookings");
            }
            catch (Exception ex)
            {
                // Return detailed error information for debugging
                return Error($"Error in test recent bookings: {ex.Message}. Inner: {ex.InnerException?.Message}. StackTrace: {ex.StackTrace}", 500);
            }
        }

        /// <summary>
        /// Test endpoint for station utilization - any authenticated user
        /// </summary>
        [HttpGet("test-station-utilization")]
        [Authorize]
        public async Task<IActionResult> GetStationUtilizationTest()
        {
            var query = new GetStationUtilizationQuery();
            var result = await _mediator.Send(query);
            return Success(result);
        }
    }
}