using Application.DTOs.Response.EVOwners;
using MediatR;

namespace Application.UseCases.EVOwners.Commands
{
    public class ActivateEVOwnerCommand : IRequest<EVOwnerResponseDto>
    {
        public string NIC { get; set; } = string.Empty;
    }
}
