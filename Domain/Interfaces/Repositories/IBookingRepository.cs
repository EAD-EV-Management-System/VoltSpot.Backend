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
        Task<List<Booking>> GetAllAsync(int page = 1, int pageSize = 50, string? status = null, string? evOwnerNic = null, string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null);
        
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

        // ✅ NEW: Get bookings by station and date range for slot availability checking
        Task<List<Booking>> GetBookingsByStationAndDateRangeAsync(string stationId, DateTime startDate, DateTime endDate);
        
        // ✅ NEW: Get overlapping bookings for validation
        Task<List<Booking>> GetOverlappingBookingsAsync(string stationId, int slotNumber, DateTime startTime, DateTime endTime);
    }
}
