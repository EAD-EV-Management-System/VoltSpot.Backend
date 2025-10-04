
using System;

namespace VoltSpot.Application.DTOs
{
    // Request to create new booking
    public class CreateBookingRequestDto
    {
        public string EvOwnerNic { get; set; } = string.Empty;
        public string ChargingStationId { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public DateTime ReservationDateTime { get; set; }
    }

    // Request to update booking
    public class UpdateBookingRequestDto
    {
        public string BookingId { get; set; } = string.Empty;
        public DateTime NewReservationDateTime { get; set; }
    }

    // Request to cancel booking - UPDATED to include CancellationReason
    public class CancelBookingRequestDto
    {
        public string BookingId { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
    }

    // Response for all booking operations
    public class BookingResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BookingId { get; set; }
        public string? QrCode { get; set; }
        public string? Status { get; set; }
    }

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