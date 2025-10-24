using Application.DTOs.Response.EVOwners;
using MediatR;

namespace Application.UseCases.EVOwners.Commands
{
    public class DeactivateEVOwnerCommand : IRequest<EVOwnerResponseDto>
    {
        public string NIC { get; set; } = string.Empty;
    }
}
