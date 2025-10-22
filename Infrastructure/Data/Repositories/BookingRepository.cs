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

        // ✅ NEW: Get bookings by station and date range for slot availability checking
        public async Task<List<Booking>> GetBookingsByStationAndDateRangeAsync(string stationId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.ChargingStationId, stationId),
                Builders<Booking>.Filter.Gte(b => b.ReservationDateTime, startDate),
                Builders<Booking>.Filter.Lt(b => b.ReservationDateTime, endDate),
                Builders<Booking>.Filter.Or(
                    Builders<Booking>.Filter.Eq(b => b.Status, BookingStatus.Confirmed),
                    Builders<Booking>.Filter.Eq(b => b.Status, BookingStatus.Pending)
                )
            );

            return await _collection.Find(filter)
                .SortBy(b => b.ReservationDateTime)
                .ToListAsync();
        }

        // ✅ NEW: Get overlapping bookings for validation
        public async Task<List<Booking>> GetOverlappingBookingsAsync(string stationId, int slotNumber, DateTime startTime, DateTime endTime)
        {
            var bookings = await _collection.Find(b =>
                b.ChargingStationId == stationId &&
                b.SlotNumber == slotNumber &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
                .ToListAsync();

            // Filter for actual time overlap
            // A booking overlaps if it starts before our end time AND ends after our start time
            var overlapping = bookings.Where(b =>
            {
                // Assuming default 2 hour duration if not specified
                var bookingEndTime = b.ReservationDateTime.AddHours(2);
                return b.ReservationDateTime < endTime && bookingEndTime > startTime;
            }).ToList();

            return overlapping;
        }
    }
}
