using System;

namespace VoltSpot.Application.DTOs.Response.Bookings
{
    public class BookingCountsDto
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int UpcomingCount { get; set; }
    }
}
