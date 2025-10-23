namespace Application.DTOs.Response.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalBookings { get; set; }
        public int TotalStations { get; set; }
        public int TotalEVOwners { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int ActiveStations { get; set; }
        public int InactiveStations { get; set; }
        public double AverageUtilization { get; set; }
    }
}