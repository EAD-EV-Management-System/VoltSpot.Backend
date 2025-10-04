
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities
{
    public class Booking
    {
        public string Id { get; private set; }
        public string EvOwnerNic { get; private set; }
        public string ChargingStationId { get; private set; }
        public int SlotNumber { get; private set; }
        public DateTime BookingDate { get; private set; }
        public DateTime ReservationDateTime { get; private set; }
        public BookingStatus Status { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string QrCode { get; set; }

        // Private constructor for EF Core
        private Booking(string evOwnerNic) { }

       
        /// Creates a new booking reservation
        
        public Booking(string evOwnerNic, string chargingStationId, int slotNumber, DateTime reservationDateTime)
        {
            // Validate reservation date is within 7 days
            if (reservationDateTime <= DateTime.UtcNow)
                throw new InvalidReservationException("Reservation date must be in the future");

            if (reservationDateTime > DateTime.UtcNow.AddDays(7))
                throw new InvalidReservationException("Reservation date must be within 7 days from today");

            Id = Guid.NewGuid().ToString();
            EvOwnerNic = evOwnerNic ?? throw new ArgumentNullException(nameof(evOwnerNic));
            ChargingStationId = chargingStationId ?? throw new ArgumentNullException(nameof(chargingStationId));
            SlotNumber = slotNumber;
            BookingDate = DateTime.UtcNow;
            ReservationDateTime = reservationDateTime;
            Status = BookingStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        
        /// Updates the reservation date/time with business rules validation
       
        public void UpdateReservation(DateTime newReservationDateTime)
        {
            // Must be at least 12 hours before current reservation
            if (DateTime.UtcNow.AddHours(12) > ReservationDateTime)
                throw new InvalidReservationException("Cannot update reservation within 12 hours of scheduled time");

            // New date must be within 7 days
            if (newReservationDateTime > DateTime.UtcNow.AddDays(7))
                throw new InvalidReservationException("New reservation date must be within 7 days from today");

            if (newReservationDateTime <= DateTime.UtcNow)
                throw new InvalidReservationException("New reservation date must be in the future");

            if (Status != BookingStatus.Pending && Status != BookingStatus.Confirmed)
                throw new InvalidReservationException($"Cannot update booking with status: {Status}");

            ReservationDateTime = newReservationDateTime;
            UpdatedAt = DateTime.UtcNow;
            Status = BookingStatus.Pending; // Reset to pending for re-approval
        }

       
        /// Cancels the booking with reason
        
        public void Cancel(string reason)
        {
            // Must be at least 12 hours before reservation
            if (DateTime.UtcNow.AddHours(12) > ReservationDateTime)
                throw new InvalidReservationException("Cannot cancel reservation within 12 hours of scheduled time");

            if (Status == BookingStatus.Cancelled || Status == BookingStatus.Completed)
                throw new InvalidReservationException($"Cannot cancel booking with status: {Status}");

            Status = BookingStatus.Cancelled;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        
        /// Confirms the booking (used by station operators)
        
        public void Confirm()
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidReservationException($"Can only confirm pending bookings. Current status: {Status}");

            Status = BookingStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        
        /// Marks booking as completed
        
        public void Complete()
        {
            if (Status != BookingStatus.Confirmed)
                throw new InvalidReservationException($"Can only complete confirmed bookings. Current status: {Status}");

            Status = BookingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

       
        /// Checks if booking can be modified
        
        public bool CanBeModified()
        {
            return DateTime.UtcNow.AddHours(12) <= ReservationDateTime &&
                   (Status == BookingStatus.Pending || Status == BookingStatus.Confirmed);
        }
    }
}