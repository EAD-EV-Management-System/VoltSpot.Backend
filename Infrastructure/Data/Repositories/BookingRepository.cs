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
