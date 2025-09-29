using Application.DTOs.Request.Auth;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Controllers.Base;

namespace WebAPI.Controllers.V1
{
    public class AuthController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _authenticationService.LoginAsync(request);
                return Success(response, "Login successful");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Error(ex.Message, 401);
            }
            catch (Exception ex)
            {
                return Error($"Login failed: {ex.Message}", 500);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var response = await _authenticationService.RefreshTokenAsync(request);
                return Success(response, "Token refreshed successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Error(ex.Message, 401);
            }
            catch (Exception ex)
            {
                return Error($"Token refresh failed: {ex.Message}", 500);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Error("Invalid user context", 400);
                }

                var result = await _authenticationService.LogoutAsync(userId);
                return result ? Success("Logout successful") : Error("Logout failed", 500);
            }
            catch (Exception ex)
            {
                return Error($"Logout failed: {ex.Message}", 500);
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userInfo = new
                {
                    Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Username = User.FindFirst(ClaimTypes.Name)?.Value,
                    Email = User.FindFirst(ClaimTypes.Email)?.Value,
                    FirstName = User.FindFirst(ClaimTypes.GivenName)?.Value,
                    LastName = User.FindFirst(ClaimTypes.Surname)?.Value,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value,
                    Status = User.FindFirst("status")?.Value,
                    AssignedStations = User.FindAll("assignedStation").Select(c => c.Value).ToList()
                };

                return Success(userInfo, "User information retrieved successfully");
            }
            catch (Exception ex)
            {
                return Error($"Failed to retrieve user information: {ex.Message}", 500);
            }
        }
    }
}
