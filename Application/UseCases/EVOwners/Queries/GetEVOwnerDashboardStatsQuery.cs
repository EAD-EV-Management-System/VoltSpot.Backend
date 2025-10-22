using Application.DTOs.Response.EVOwners;
using MediatR;

namespace Application.UseCases.EVOwners.Queries
{
    public class GetEVOwnerDashboardStatsQuery : IRequest<EVOwnerDashboardStatsDto>
    {
        public string EvOwnerNic { get; set; } = string.Empty;
    }
}
