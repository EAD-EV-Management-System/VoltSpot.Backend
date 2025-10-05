using System;

namespace VoltSpot.Application.DTOs
{
    // Request to cancel booking - UPDATED to include CancellationReason
    public class CancelBookingRequestDto
    {
        public string BookingId { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
    }
}

