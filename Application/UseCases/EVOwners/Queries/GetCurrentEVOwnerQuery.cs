using Application.DTOs.Response.EVOwners;
using MediatR;

namespace Application.UseCases.EVOwners.Queries
{
    public class GetCurrentEVOwnerQuery : IRequest<EVOwnerResponseDto>
    {
        public string EvOwnerNic { get; set; } = string.Empty;
    }
}