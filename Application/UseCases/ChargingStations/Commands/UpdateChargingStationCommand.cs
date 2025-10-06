using Application.DTOs.Response.ChargingStations;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.ChargingStations.Commands
{
    public class UpdateChargingStationCommand : IRequest<ChargingStationResponseDto>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ChargingType Type { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public decimal PricePerHour { get; set; }
        public CommandOperatingHoursDto OperatingHours { get; set; } = new();
        public List<string> AssignedOperatorIds { get; set; } = new();
    }
}