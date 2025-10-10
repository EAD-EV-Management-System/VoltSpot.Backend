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

        // New method for admin view of all bookings
        Task<List<Booking>> GetAllAsync(int page = 1, int pageSize = 50, string? status = null, string? evOwnerNic = null, DateTime? fromDate = null, DateTime? toDate = null);
        
        // Additional methods for dashboard
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetRecentBookingsAsync(int count = 10);
        Task<List<Booking>> GetBookingsByStationAsync(string stationId);

        Task<int> GetPendingCountAsync(string evOwnerNic);
        Task<int> GetApprovedCountAsync(string evOwnerNic);
        Task<int> GetUpcomingCountAsync(string evOwnerNic);
        Task<List<Booking>> GetUpcomingBookingsAsync(string evOwnerNic);
        Task<List<Booking>> GetCompletedBookingsAsync(string evOwnerNic);
        Task<List<Booking>> GetCancelledBookingsAsync(string evOwnerNic);
    }
}
