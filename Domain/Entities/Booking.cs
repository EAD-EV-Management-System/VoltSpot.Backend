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
        public BookingStatus Status { get; set; }
        public string? CancellationReason { get; set; }
        public string QrCode { get; set; } = string.Empty;

        public void UpdateReservation(DateTime newReservationDateTime)
        {
            ReservationDateTime = newReservationDateTime;
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