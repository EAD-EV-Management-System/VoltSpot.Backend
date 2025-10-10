using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using MongoDB.Driver;
using VoltSpot.Domain.Interfaces;

namespace VoltSpot.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context.Bookings) { }

        public async Task<List<Booking>> GetBookingsByEvOwnerAsync(string evOwnerNic)
        {
            return await _collection.Find(b => b.EvOwnerNic == evOwnerNic)
                .SortByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsSlotAvailableAsync(string chargingStationId, int slotNumber, DateTime reservationDateTime)
        {
            var existingBooking = await _collection.Find(b =>
                b.ChargingStationId == chargingStationId &&
                b.SlotNumber == slotNumber &&
                b.ReservationDateTime == reservationDateTime &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                .FirstOrDefaultAsync();

            return existingBooking == null;
        }

        // Get all bookings without pagination for dashboard statistics
        public async Task<List<Booking>> GetAllAsync()
        {
            return await _collection.Find(_ => true)
                .SortByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        // New method for admin view of all bookings with pagination and filtering
        public async Task<List<Booking>> GetAllAsync(int page = 1, int pageSize = 50, string? status = null, string? evOwnerNic = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var filter = Builders<Booking>.Filter.Empty;

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, true, out var bookingStatus))
            {
                filter &= Builders<Booking>.Filter.Eq(b => b.Status, bookingStatus);
            }

            // Apply EV Owner NIC filter
            if (!string.IsNullOrEmpty(evOwnerNic))
            {
                filter &= Builders<Booking>.Filter.Eq(b => b.EvOwnerNic, evOwnerNic);
            }

            // Apply date range filter
            if (fromDate.HasValue)
            {
                filter &= Builders<Booking>.Filter.Gte(b => b.ReservationDateTime, fromDate.Value);
            }

            if (toDate.HasValue)
            {
                filter &= Builders<Booking>.Filter.Lte(b => b.ReservationDateTime, toDate.Value);
            }

            return await _collection.Find(filter)
                .SortByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        // Get recent bookings for dashboard
        public async Task<List<Booking>> GetRecentBookingsAsync(int count = 10)
        {
            return await _collection.Find(_ => true)
                .SortByDescending(b => b.CreatedAt)
                .Limit(count)
                .ToListAsync();
        }

        // Get bookings by station for utilization calculations
        public async Task<List<Booking>> GetBookingsByStationAsync(string stationId)
        {
            return await _collection.Find(b => b.ChargingStationId == stationId)
                .SortByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        // ✅ New repository methods
        public async Task<int> GetPendingCountAsync(string evOwnerNic)
        {
            return (int)await _collection.CountDocumentsAsync(b =>
                b.EvOwnerNic == evOwnerNic && b.Status == BookingStatus.Pending);
        }

        public async Task<int> GetApprovedCountAsync(string evOwnerNic)
        {
            return (int)await _collection.CountDocumentsAsync(b =>
                b.EvOwnerNic == evOwnerNic && b.Status == BookingStatus.Confirmed);
        }

        public async Task<int> GetUpcomingCountAsync(string evOwnerNic)
        {
            return (int)await _collection.CountDocumentsAsync(b =>
                b.EvOwnerNic == evOwnerNic &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) &&
                b.ReservationDateTime >= DateTime.UtcNow);
        }

        public async Task<List<Booking>> GetUpcomingBookingsAsync(string evOwnerNic)
        {
            return await _collection.Find(b =>
                b.EvOwnerNic == evOwnerNic &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                b.ReservationDateTime >= DateTime.UtcNow)
                .SortBy(b => b.ReservationDateTime)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetCompletedBookingsAsync(string evOwnerNic)
        {
            return await _collection.Find(b =>
                b.EvOwnerNic == evOwnerNic && b.Status == BookingStatus.Completed)
                .SortByDescending(b => b.ReservationDateTime)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetCancelledBookingsAsync(string evOwnerNic)
        {
            return await _collection.Find(b =>
                b.EvOwnerNic == evOwnerNic && b.Status == BookingStatus.Cancelled)
                .SortByDescending(b => b.ReservationDateTime)
                .ToListAsync();
        }
    }
}
