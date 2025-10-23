using Application.DTOs.Response.ChargingStations;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.ChargingStations.Queries
{
    public class SearchChargingStationsQuery : IRequest<List<ChargingStationResponseDto>>
    {
        public string? Location { get; set; }
        public ChargingType? Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusKm { get; set; } = 10; // Default 10km radius
        public bool? AvailableOnly { get; set; } = true;
        public decimal? MaxPricePerHour { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}