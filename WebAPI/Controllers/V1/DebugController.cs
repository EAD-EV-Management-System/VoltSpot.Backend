using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Base;
using System.Security.Claims;
using Domain.Interfaces.Repositories;
using VoltSpot.Domain.Interfaces;

namespace WebAPI.Controllers.V1
{
    public class DebugController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _stationRepository;

        public DebugController(
            IUserRepository userRepository, 
            IEVOwnerRepository evOwnerRepository,
            IBookingRepository bookingRepository,
            IChargingStationRepository stationRepository)
        {
            _userRepository = userRepository;
            _evOwnerRepository = evOwnerRepository;
            _bookingRepository = bookingRepository;
            _stationRepository = stationRepository;
        }

        /// <summary>
        /// Debug endpoint to see current user claims and roles
        /// </summary>
        [HttpGet("token-info")]
        [Authorize]
        public async Task<IActionResult> GetTokenInfo()
        {
            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            var userInfo = new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Name = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                NIC = User.FindFirst("nic")?.Value,
                AllClaims = claims
            };

            return Success(userInfo, "Token information retrieved successfully");
        }

        /// <summary>
        /// Debug endpoint to check database counts
        /// </summary>
        [HttpGet("database-counts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDatabaseCounts()
        {
            try
            {
                var allUsers = await _userRepository.GetAllAsync();
                var allEVOwners = await _evOwnerRepository.GetAllAsync();
                var allBookings = await _bookingRepository.GetAllAsync();
                var allStations = await _stationRepository.GetAllAsync();

                var counts = new
                {
                    TotalUsers = allUsers.Count(),
                    TotalEVOwners = allEVOwners.Count(),
                    TotalBookings = allBookings.Count,
                    TotalStations = allStations.Count(),
                    UsersByRole = allUsers.GroupBy(u => u.Role).Select(g => new { Role = g.Key.ToString(), Count = g.Count() }),
                    EVOwnersByStatus = allEVOwners.GroupBy(e => e.Status).Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                };

                return Success(counts, "Database counts retrieved successfully");
            }
            catch (Exception ex)
            {
                return Error($"Error retrieving database counts: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Test recent bookings step by step
        /// </summary>
        [HttpGet("test-recent-bookings-debug")]
        [AllowAnonymous]
        public async Task<IActionResult> TestRecentBookingsDebug()
        {
            try
            {
                // Step 1: Test booking repository
                var recentBookings = await _bookingRepository.GetRecentBookingsAsync(5);
                
                if (recentBookings == null || !recentBookings.Any())
                {
                    return Success(new { 
                        Step = "Booking Repository", 
                        Result = "No bookings found",
                        BookingCount = 0
                    }, "No bookings in database");
                }

                var debugInfo = new List<object>();

                // Step 2: Process each booking
                foreach (var booking in recentBookings.Take(3)) // Just test first 3
                {
                    try
                    {
                        var station = await _stationRepository.GetByIdAsync(booking.ChargingStationId);
                        var evOwner = await _evOwnerRepository.GetByNICAsync(booking.EvOwnerNic);

                        debugInfo.Add(new
                        {
                            BookingId = booking.Id,
                            EvOwnerNic = booking.EvOwnerNic,
                            StationId = booking.ChargingStationId,
                            StationFound = station != null,
                            StationName = station?.Name ?? "Not found",
                            EvOwnerFound = evOwner != null,
                            EvOwnerName = evOwner != null ? $"{evOwner.FirstName} {evOwner.LastName}".Trim() : "Not found",
                            Status = booking.Status.ToString(),
                            CreatedAt = booking.CreatedAt
                        });
                    }
                    catch (Exception ex)
                    {
                        debugInfo.Add(new
                        {
                            BookingId = booking.Id,
                            Error = ex.Message,
                            StackTrace = ex.StackTrace
                        });
                    }
                }

                return Success(new
                {
                    TotalBookings = recentBookings.Count,
                    ProcessedBookings = debugInfo
                }, "Recent bookings debug completed successfully");

            }
            catch (Exception ex)
            {
                return Error($"Error in debug: {ex.Message}. StackTrace: {ex.StackTrace}", 500);
            }
        }

        /// <summary>
        /// Test endpoint to get users without role restriction
        /// </summary>
        [HttpGet("users-test")]
        [Authorize]
        public async Task<IActionResult> GetUsersTest()
        {
            try
            {
                var allUsers = await _userRepository.GetAllAsync();
                return Success(allUsers, $"Found {allUsers.Count()} users in database");
            }
            catch (Exception ex)
            {
                return Error($"Error retrieving users: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Test endpoint that requires no authentication
        /// </summary>
        [HttpGet("ping")]
        [AllowAnonymous]
        public async Task<IActionResult> Ping()
        {
            return Success(new { 
                Message = "Pong", 
                Timestamp = DateTime.UtcNow,
                Server = "VoltSpot Backend"
            }, "Server is running");
        }
    }
}