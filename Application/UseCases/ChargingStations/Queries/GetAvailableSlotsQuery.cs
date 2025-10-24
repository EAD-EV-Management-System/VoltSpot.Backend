using Application.DTOs.Response.ChargingStations;
using MediatR;

namespace Application.UseCases.ChargingStations.Queries
{
    public class GetAvailableSlotsQuery : IRequest<AvailableSlotsDto>
    {
        public string StationId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan? Time { get; set; }
        public int DurationInMinutes { get; set; } = 120;
    }
}
