using Application.DTOs.Response.Dashboard;
using MediatR;

namespace Application.UseCases.Dashboard.Queries
{
    public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
    }
}