using Application.Interfaces.Services;
using Application.UseCases.Users.Commands;
using Domain.Enums;
using MediatR;
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
    }
}
