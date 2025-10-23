using Application.DTOs.Request.EVOwners;
using Application.UseCases.EVOwners.Commands;
using Application.UseCases.EVOwners.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Base;

namespace WebAPI.Controllers.V1
{
    public class EVOwnersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public EVOwnersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterEVOwnerRequestDto request)
        {
            var command = _mapper.Map<RegisterEVOwnerCommand>(request);
            var result = await _mediator.Send(command);
            return Success(result, "EV Owner registered successfully");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] EVOwnerLoginRequestDto request)
        {
            var command = _mapper.Map<LoginEVOwnerCommand>(request);
            var result = await _mediator.Send(command);
            return Success(result, "Login successful");
        }

        // ? FIXED: Get all EV owners for admin management - now uses correct roles
        [HttpGet]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> GetAllEVOwners(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? status = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetAllEVOwnersQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                SearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);
            return Success(result);
        }

        // ? NEW: Get dashboard statistics for an EV owner
        [HttpGet("{nic}/dashboard-stats")]
        [Authorize]
        public async Task<IActionResult> GetDashboardStats(string nic)
        {
            var query = new GetEVOwnerDashboardStatsQuery { EvOwnerNic = nic };
            var result = await _mediator.Send(query);
            return Success(result, "Dashboard statistics retrieved successfully");
        }

        // ? NEW: Get current EV owner profile
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentEVOwner()
        {
            var evOwnerNic = User.Claims.FirstOrDefault(c => c.Type == "nic")?.Value;
            
            if (string.IsNullOrEmpty(evOwnerNic))
            {
                return Unauthorized("User not authenticated properly");
            }

            var query = new GetCurrentEVOwnerQuery { EvOwnerNic = evOwnerNic };
            var result = await _mediator.Send(query);
            return Success(result);
        }

        // ? NEW: Update current EV owner profile
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentEVOwner([FromBody] UpdateEVOwnerRequestDto request)
        {
            var evOwnerNic = User.Claims.FirstOrDefault(c => c.Type == "nic")?.Value;
            
            if (string.IsNullOrEmpty(evOwnerNic))
            {
                return Unauthorized("User not authenticated properly");
            }

            var command = _mapper.Map<UpdateEVOwnerCommand>(request);
            command.NIC = evOwnerNic; // Ensure we're updating the current user

            var result = await _mediator.Send(command);
            return Success(result, "Profile updated successfully");
        }

        // ? FIXED: Update any EV owner by admin - now uses correct roles
        [HttpPut("{nic}")]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> UpdateEVOwner(string nic, [FromBody] UpdateEVOwnerRequestDto request)
        {
            var command = _mapper.Map<UpdateEVOwnerCommand>(request);
            command.NIC = nic;

            var result = await _mediator.Send(command);
            return Success(result, "EV Owner updated successfully");
        }

        // ? FIXED: Get specific EV owner by NIC - now uses correct roles
        [HttpGet("{nic}")]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> GetEVOwnerByNic(string nic)
        {
            var query = new GetCurrentEVOwnerQuery { EvOwnerNic = nic };
            var result = await _mediator.Send(query);
            return Success(result);
        }

        // ? FIXED: Delete EV owner - now uses correct roles
        [HttpDelete("{nic}")]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> DeleteEVOwner(string nic)
        {
            var command = new DeleteEVOwnerCommand { NIC = nic };
            var result = await _mediator.Send(command);
            return Success("EV Owner deleted successfully");
        }

        // ? TEST ENDPOINT (Remove after testing)
        /// <summary>
        /// Test endpoint for getting all EV owners - any authenticated user
        /// </summary>
        [HttpGet("test")]
        [Authorize]
        public async Task<IActionResult> GetAllEVOwnersTest(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? status = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetAllEVOwnersQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                SearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);
            return Success(result);
        }
    }
}
