using System;

namespace VoltSpot.Application.DTOs
{
    // Detailed booking information
    public class BookingDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string EvOwnerNic { get; set; } = string.Empty;
        public string ChargingStationId { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? QrCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}