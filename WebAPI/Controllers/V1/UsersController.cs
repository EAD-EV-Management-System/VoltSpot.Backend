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
            try
            {
                var users = await _mediator.Send(query);
                return Success(users, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return Error($"Failed to retrieve users: {ex.Message}", 500);
            }
        }

        [HttpGet("{id}")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
                if (user == null)
                {
                    return Error("User not found", 404);
                }

                return Success(user, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return Error($"Failed to retrieve user: {ex.Message}", 500);
            }
        }

        [HttpPost]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            try
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
            catch (InvalidOperationException ex)
            {
                return Error(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return Error($"Failed to create user: {ex.Message}", 500);
            }
        }

        [HttpPut("{id}")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequestDto request)
        {
            try
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
            catch (InvalidOperationException ex)
            {
                return Error(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return Error($"Failed to update user: {ex.Message}", 500);
            }
        }

        [HttpDelete("{id}")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var command = new DeleteUserCommand
                {
                    Id = id,
                    DeletedBy = currentUserId ?? ""
                };

                var result = await _mediator.Send(command);
                return result ? Success("User deleted successfully") : Error("User not found", 404);
            }
            catch (Exception ex)
            {
                return Error($"Failed to delete user: {ex.Message}", 500);
            }
        }

        [HttpPost("{id}/assign-stations")]
        [RequireRole("Backoffice")]
        public async Task<IActionResult> AssignStations(string id, [FromBody] List<string> stationIds)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
                if (user == null)
                {
                    return Error("User not found", 404);
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
            catch (Exception ex)
            {
                return Error($"Failed to assign stations: {ex.Message}", 500);
            }
        }
    }
}
