using Application.DTOs.Request.EVOwners;
using Application.UseCases.EVOwners.Commands;
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
    }
}
