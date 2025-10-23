namespace Application.DTOs.Response.Bookings
{
    public class BookingValidationDto
    {
        public bool IsValid { get; set; }
        public bool IsAvailable { get; set; }
        public bool CanBook { get; set; }
        public List<string> ValidationMessages { get; set; } = new();
        public decimal? EstimatedCost { get; set; }
        public BookingSuggestionsDto? Suggestions { get; set; }
    }

    public class BookingSuggestionsDto
    {
        public List<int> AlternativeSlots { get; set; } = new();
        public List<AlternativeTimeDto> AlternativeTimes { get; set; } = new();
        public List<NearbyStationDto> NearbyStations { get; set; } = new();
    }

    public class AlternativeTimeDto
    {
        public string Time { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public bool Available { get; set; }
    }

    public class NearbyStationDto
    {
        public string StationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public double Distance { get; set; }
        public List<int> AvailableSlots { get; set; } = new();
    }
}
