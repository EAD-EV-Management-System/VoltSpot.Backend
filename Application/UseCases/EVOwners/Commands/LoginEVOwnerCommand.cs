using Application.DTOs.Response.EVOwners;
using MediatR;

namespace Application.UseCases.EVOwners.Commands
{
    public class LoginEVOwnerCommand : IRequest<EVOwnerLoginResponseDto>
    {
        public string NIC { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
