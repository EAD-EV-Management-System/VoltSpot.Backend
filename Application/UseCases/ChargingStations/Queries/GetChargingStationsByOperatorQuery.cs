using Application.DTOs.Response.ChargingStations;
using MediatR;

namespace Application.UseCases.ChargingStations.Queries
{
    public class GetChargingStationsByOperatorQuery : IRequest<List<ChargingStationResponseDto>>
    {
        public string OperatorId { get; set; } = string.Empty;
    }
}
