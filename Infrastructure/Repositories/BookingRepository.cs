
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Domain.Enums;
using Domain.Entities;
using VoltSpot.Domain.Interfaces;

namespace VoltSpot.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IMongoCollection<Booking> _bookings;

        public BookingRepository(IMongoDatabase database)
        {
            _bookings = database.GetCollection<Booking>("Bookings");
        }

        /// <summary>
        /// Saves a new booking to MongoDB
        /// </summary>
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            await _bookings.InsertOneAsync(booking);
            return booking;
        }

        /// <summary>
        /// Gets a booking by ID from MongoDB
        /// </summary>
        public async Task<Booking> GetBookingByIdAsync(string bookingId)
        {
            return await _bookings.Find(b => b.Id == bookingId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all bookings for a specific EV owner
        /// </summary>
        public async Task<List<Booking>> GetBookingsByEvOwnerAsync(string evOwnerNic)
        {
            return await _bookings.Find(b => b.EvOwnerNic == evOwnerNic)
                                 .SortByDescending(b => b.CreatedAt)
                                 .ToListAsync();
        }

        /// <summary>
        /// Updates an existing booking in MongoDB
        /// </summary>
        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            await _bookings.ReplaceOneAsync(b => b.Id == booking.Id, booking);
            return booking;
        }

        /// <summary>
        /// Checks if a slot is available at specific time
        /// </summary>
        public async Task<bool> IsSlotAvailableAsync(string chargingStationId, int slotNumber, DateTime reservationDateTime)
        {
            // Check if there's any approved booking for the same slot at the same time
            var existingBooking = await _bookings.Find(b =>
                b.ChargingStationId == chargingStationId &&
                b.SlotNumber == slotNumber &&
                b.ReservationDateTime == reservationDateTime &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                .FirstOrDefaultAsync();

            return existingBooking == null; // Available if no existing booking found
        }
    }
}