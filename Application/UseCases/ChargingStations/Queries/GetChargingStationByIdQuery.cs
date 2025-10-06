using Application.DTOs.Response.ChargingStations;
using MediatR;

namespace Application.UseCases.ChargingStations.Queries
{
    public class GetChargingStationByIdQuery : IRequest<ChargingStationResponseDto?>
    {
        public string Id { get; set; } = string.Empty;
    }
}