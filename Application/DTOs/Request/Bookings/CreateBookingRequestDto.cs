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

}