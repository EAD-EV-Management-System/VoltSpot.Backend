using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string EvOwnerNic { get; set; } = string.Empty;
        public string ChargingStationId { get; set; } = string.Empty;
        public int SlotNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public int DurationInMinutes { get; set; } = 120; // Default 2 hours (120 minutes)
        public BookingStatus Status { get; set; }
        public string? CancellationReason { get; set; }
        public string QrCode { get; set; } = string.Empty;

        /// <summary>
        /// Calculates the end time of the booking based on reservation time and duration
        /// </summary>
        public DateTime GetEndDateTime()
        {
            return ReservationDateTime.AddMinutes(DurationInMinutes);
        }

        /// <summary>
        /// Checks if this booking overlaps with a given time period
        /// </summary>
        public bool OverlapsWith(DateTime startTime, DateTime endTime)
        {
            var bookingEndTime = GetEndDateTime();
            return ReservationDateTime < endTime && bookingEndTime > startTime;
        }

        public void UpdateReservation(DateTime newReservationDateTime)
        {
            ReservationDateTime = newReservationDateTime;
            UpdatedAt = DateTime.UtcNow;
            Status = BookingStatus.Pending;
        }

        public void UpdateReservation(DateTime newReservationDateTime, int newDurationInMinutes)
        {
            ReservationDateTime = newReservationDateTime;
            DurationInMinutes = newDurationInMinutes;
            UpdatedAt = DateTime.UtcNow;
            Status = BookingStatus.Pending;
        }

        public void Cancel(string reason)
        {
            Status = BookingStatus.Cancelled;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm()
        {
            Status = BookingStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            Status = BookingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool CanBeModified()
        {
            return DateTime.UtcNow.AddHours(12) <= ReservationDateTime &&
                   (Status == BookingStatus.Pending || Status == BookingStatus.Confirmed);
        }
    }
}