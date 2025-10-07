using Application.DTOs.Response.ChargingStations;
using MediatR;

namespace Application.UseCases.ChargingStations.Queries
{
    public class GetAllChargingStationsQuery : IRequest<List<ChargingStationResponseDto>>
    {
        public bool OnlyActive { get; set; } = false;
    }
}