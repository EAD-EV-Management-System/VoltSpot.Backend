
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
        public BookingRepository(AppDbContext context) : base(context.Bookings)
        {
        }

        public async Task<List<Booking>> GetBookingsByEvOwnerAsync(string evOwnerNic)
        {
            return await _collection.Find(b => b.EvOwnerNic == evOwnerNic)
                                 .SortByDescending(b => b.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<bool> IsSlotAvailableAsync(string chargingStationId, int slotNumber, DateTime reservationDateTime)
        {
            // Check if there's any approved booking for the same slot at the same time
            var existingBooking = await _collection.Find(b =>
                b.ChargingStationId == chargingStationId &&
                b.SlotNumber == slotNumber &&
                b.ReservationDateTime == reservationDateTime &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                .FirstOrDefaultAsync();

            return existingBooking == null; // Available if no existing booking found
        }
    }
}