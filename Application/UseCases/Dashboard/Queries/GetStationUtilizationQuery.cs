using Application.DTOs.Response.Dashboard;
using MediatR;

namespace Application.UseCases.Dashboard.Queries
{
    public class GetStationUtilizationQuery : IRequest<List<StationUtilizationDto>>
    {
    }
}