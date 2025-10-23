using Application.Interfaces.Services;
using Application.UseCases.Users.Commands;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Base;

namespace WebAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SeedController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IPasswordService _passwordService;

        public SeedController(IMediator mediator, IPasswordService passwordService)
        {
            _mediator = mediator;
            _passwordService = passwordService;
        }

        [HttpPost("create-admin")]
        [AllowAnonymous] // ✅ Allow anonymous access for testing
        public async Task<IActionResult> CreateAdmin()
        {
            try
            {
                var command = new CreateUserCommand
                {
                    Username = "admin",
                    Email = "admin@voltspot.com",
                    Password = "Admin123!",
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = UserRole.Backoffice,
                    CreatedBy = "system"
                };

                var admin = await _mediator.Send(command);
                return Success(admin, "Admin user created successfully");
            }
            catch (InvalidOperationException ex)
            {
                return Error(ex.Message, 400);
            }
        }

        [HttpPost("create-test-users")]
        [AllowAnonymous] // ✅ Create multiple test users
        public async Task<IActionResult> CreateTestUsers()
        {
            try
            {
                var users = new List<object>();

                // Create Backoffice admin
                try
                {
                    var adminCommand = new CreateUserCommand
                    {
                        Username = "admin",
                        Email = "admin@voltspot.com",
                        Password = "Admin123!",
                        FirstName = "System",
                        LastName = "Administrator",
                        Role = UserRole.Backoffice,
                        CreatedBy = "system"
                    };
                    var admin = await _mediator.Send(adminCommand);
                    users.Add(new { Type = "Admin", User = admin });
                }
                catch (InvalidOperationException)
                {
                    users.Add(new { Type = "Admin", Message = "Admin user already exists" });
                }

                // Create Station Operator
                try
                {
                    var operatorCommand = new CreateUserCommand
                    {
                        Username = "operator1",
                        Email = "operator@voltspot.com",
                        Password = "Operator123!",
                        FirstName = "John",
                        LastName = "Operator",
                        Role = UserRole.StationOperator,
                        CreatedBy = "system",
                        AssignedStationIds = new List<string>()
                    };
                    var operatorUser = await _mediator.Send(operatorCommand);
                    users.Add(new { Type = "Operator", User = operatorUser });
                }
                catch (InvalidOperationException)
                {
                    users.Add(new { Type = "Operator", Message = "Operator user already exists" });
                }

                return Success(users, $"Test users created/checked successfully. Created {users.Count} users.");
            }
            catch (Exception ex)
            {
                return Error($"Error creating test users: {ex.Message}", 500);
            }
        }
    }
}
