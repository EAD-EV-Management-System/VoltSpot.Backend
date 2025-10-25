using Application.DTOs.Request.Auth;
using Application.UseCases.Auth.Commands;
using Application.UseCases.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Controllers.Base;

namespace WebAPI.Controllers.V1
{
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                Username = request.Username,
                Password = request.Password
            };

            var response = await _mediator.Send(command);
            return Success(response, "Login successful");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            };

            var response = await _mediator.Send(command);
            return Success(response, "Token refreshed successfully");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid user context");
            }

            var command = new LogoutCommand { UserId = userId };
            var result = await _mediator.Send(command);

            return result ? Success("Logout successful") : throw new InvalidOperationException("Logout failed");
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid user context");
            }

            var query = new GetCurrentUserQuery { UserId = userId };
            var userInfo = await _mediator.Send(query);

            return Success(userInfo, "User information retrieved successfully");
        }

        // ? DIAGNOSTIC ENDPOINT - Verify token claims
        /// <summary>
        /// Test endpoint to verify all claims in the JWT token
        /// </summary>
        [HttpGet("verify-token")]
        [Authorize]
        public IActionResult VerifyToken()
        {
            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            var roleClaimValue = User.FindFirst(ClaimTypes.Role)?.Value;
            var isInBackofficeRole = User.IsInRole("Backoffice");
            var isInEVOwnerRole = User.IsInRole("EVOwner");
            var isInStationOperatorRole = User.IsInRole("StationOperator");

            return Success(new
            {
                AllClaims = claims,
                RoleClaimValue = roleClaimValue,
                IsInBackofficeRole = isInBackofficeRole,
                IsInEVOwnerRole = isInEVOwnerRole,
                IsInStationOperatorRole = isInStationOperatorRole,
                ClaimTypes = new
                {
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                    NameIdentifierClaimType = ClaimTypes.NameIdentifier
                }
            }, "Token claims verified successfully");
        }
    }
}
