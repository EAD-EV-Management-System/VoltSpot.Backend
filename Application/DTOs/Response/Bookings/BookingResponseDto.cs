using System;

namespace VoltSpot.Application.DTOs
{
    // Response for all booking operations
    public class BookingResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BookingId { get; set; }
        public string? QrCode { get; set; }
        public string? Status { get; set; }
    }
}