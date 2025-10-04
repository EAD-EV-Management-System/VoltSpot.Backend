using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoltSpot.Domain.Interfaces
{
    public interface IBookingRepository
    {
        
        /// Saves a new booking to database
       
        Task<Booking> CreateBookingAsync(Booking booking);

        
        /// Gets a booking by its ID
        
        Task<Booking> GetBookingByIdAsync(string bookingId);

        
        /// Gets all bookings for a specific EV owner
        
        Task<List<Booking>> GetBookingsByEvOwnerAsync(string evOwnerNic);

        
        /// Updates an existing booking
        
        Task<Booking> UpdateBookingAsync(Booking booking);

       
        /// Checks if a slot is available at specific time
        Task<bool> IsSlotAvailableAsync(string chargingStationId, int slotNumber, DateTime reservationDateTime);
    }
}