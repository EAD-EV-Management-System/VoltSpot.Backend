using Domain.Entities;

namespace VoltSpot.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(string id);
        Task<Booking> AddAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task<List<Booking>> GetBookingsByEvOwnerAsync(string evOwnerNic);
        Task<bool> IsSlotAvailableAsync(string chargingStationId, int slotNumber, DateTime reservationDateTime);

        //  NEW METHODS
        Task<int> GetPendingCountAsync(string evOwnerNic);
        Task<int> GetApprovedCountAsync(string evOwnerNic);
        Task<int> GetUpcomingCountAsync(string evOwnerNic);
        Task<List<Booking>> GetUpcomingBookingsAsync(string evOwnerNic);
        Task<List<Booking>> GetCompletedBookingsAsync(string evOwnerNic);
        Task<List<Booking>> GetCancelledBookingsAsync(string evOwnerNic);
    }
}
