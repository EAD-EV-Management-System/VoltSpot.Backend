using System;

namespace VoltSpot.Application.DTOs
{
    // Request to update booking
    public class UpdateBookingRequestDto
    {
        public string BookingId { get; set; } = string.Empty;
        public DateTime NewReservationDateTime { get; set; }
    }

}