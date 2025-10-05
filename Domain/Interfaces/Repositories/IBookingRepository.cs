using Domain.Entities;

namespace VoltSpot.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(string id);
        Task<Booking> AddAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);

        /// Gets all bookings for a specific EV owner
        Task<List<Booking>> GetBookingsByEvOwnerAsync(string evOwnerNic);

        /// Checks if a slot is available at specific time
        Task<bool> IsSlotAvailableAsync(string chargingStationId, int slotNumber, DateTime reservationDateTime);
    }
}