using Domain.Enums;
using System.Text.Json.Serialization;
using Application.Common.Converters;

namespace Application.DTOs.Request.ChargingStations
{
    public class UpdateChargingStationRequestDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        [JsonConverter(typeof(NullableChargingTypeJsonConverter))]
        public ChargingType? ChargingType { get; set; }
        
        [JsonConverter(typeof(ChargingTypeJsonConverter))]
        public ChargingType Type { get; set; }
        
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public decimal PricePerHour { get; set; }
        public double? PowerOutput { get; set; }
        public string? OperatorId { get; set; }
        public string? OperatingHours { get; set; } // Support string format
        public OperatingHoursDto? OperatingHoursDto { get; set; } = new();
        public List<string> AssignedOperatorIds { get; set; } = new();
        public string? Status { get; set; }
    }
}