using Application.DTOs.Request.Users;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Attributes;
using WebAPI.Controllers.Base;

namespace WebAPI.Controllers.V1
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var users = await _mediator.Send(query);
            return Success(users, "Users retrieved successfully");
        }

        // ✅ TEST ENDPOINT - Remove after debugging
        [HttpGet("test")]
        [Authorize] // Only requires authentication, no specific role
        public async Task<IActionResult> GetUsersTest([FromQuery] GetUsersQuery query)
        {
            var users = await _mediator.Send(query);
            return Success(users, $"Users retrieved successfully (Test endpoint)");
        }

        [HttpGet("{id}")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            return Success(user, "User retrieved successfully");
        }

        [HttpPost]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new CreateUserCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                AssignedStationIds = request.AssignedStationIds,
                CreatedBy = currentUserId ?? ""
            };

            var user = await _mediator.Send(command);
            return Success(user, "User created successfully");
        }

        [HttpPut("{id}")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequestDto request)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new UpdateUserCommand
            {
                Id = id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                Status = request.Status,
                AssignedStationIds = request.AssignedStationIds,
                UpdatedBy = currentUserId ?? ""
            };

            var user = await _mediator.Send(command);
            return Success(user, "User updated successfully");
        }

        [HttpDelete("{id}")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new DeleteUserCommand
            {
                Id = id,
                DeletedBy = currentUserId ?? ""
            };

            var result = await _mediator.Send(command);
            return result ? Success("User deleted successfully") : throw new KeyNotFoundException($"User with ID {id} not found");
        }

        [HttpPost("{id}/assign-stations")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> AssignStations(string id, [FromBody] List<string> stationIds)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            var command = new UpdateUserCommand
            {
                Id = id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Status = user.Status,
                AssignedStationIds = stationIds,
                UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""
            };

            var updatedUser = await _mediator.Send(command);
            return Success(updatedUser, "Stations assigned successfully");
        }
    }
}
