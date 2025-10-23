namespace Application.DTOs.Response.EVOwners
{
    public class EVOwnerDashboardStatsDto
    {
        public int PendingReservations { get; set; }
        public int ApprovedReservations { get; set; }
        public int CompletedCharges { get; set; }
        public int CancelledBookings { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public NextUpcomingBookingDto? NextUpcomingBooking { get; set; }
        public List<RecentBookingDto> RecentBookings { get; set; } = new();
    }

    public class NextUpcomingBookingDto
    {
        public string Id { get; set; } = string.Empty;
        public string ChargingStationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public DateTime ReservationDateTime { get; set; }
        public int SlotNumber { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class RecentBookingDto
    {
        public string Id { get; set; } = string.Empty;
        public string ChargingStationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public DateTime ReservationDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
