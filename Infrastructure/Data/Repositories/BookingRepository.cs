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
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context) : base(context.Bookings)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetBookingsByEvOwnerAsync(string evOwnerNic)
        {
            return await _collection.Find(b => b.EvOwnerNic == evOwnerNic)
                .SortByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByOperatorAsync(string operatorId)
        {
            // First, get all charging stations assigned to this operator
            var operatorStations = await _context.ChargingStations
                .Find(s => s.AssignedOperatorIds.Contains(operatorId))
                .ToListAsync();

            var stationIds = operatorStations.Select(s => s.Id).ToList();

            if (!stationIds.Any())
            {
                return new List<Booking>();
            }

            // Use aggregation to get bookings for these stations with ChargingStation details populated
            var pipeline = new[]
            {
                // Match bookings for the operator's stations
                new MongoDB.Bson.BsonDocument("$match", new MongoDB.Bson.BsonDocument
                {
                    { "chargingStationId", new MongoDB.Bson.BsonDocument("$in", new MongoDB.Bson.BsonArray(stationIds)) }
                }),

                // Lookup (join) with ChargingStations collection
                new MongoDB.Bson.BsonDocument("$lookup", new MongoDB.Bson.BsonDocument
                {
                    { "from", "ChargingStations" },
                    { "localField", "chargingStationId" },
                    { "foreignField", "_id" },
                    { "as", "chargingStation" }
                }),

                // Unwind the array (converts array to single object)
                new MongoDB.Bson.BsonDocument("$unwind", new MongoDB.Bson.BsonDocument
                {
                    { "path", "$chargingStation" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                // Sort by creation date descending
                new MongoDB.Bson.BsonDocument("$sort", new MongoDB.Bson.BsonDocument("createdAt", -1))
            };

            var result = await _collection.Aggregate<Booking>(pipeline).ToListAsync();
            return result;
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

        /// <summary>
        /// New method - checks for time period overlaps based on duration
        /// Properly handles Completed bookings by checking if they've ended
        /// </summary>
        public async Task<bool> IsSlotAvailableForDurationAsync(string chargingStationId, int slotNumber, DateTime startTime, int durationInMinutes)
        {
            var endTime = startTime.AddMinutes(durationInMinutes);
            var now = DateTime.UtcNow;

            // Get all bookings for this station and slot (excluding only Cancelled)
            var activeBookings = await _collection.Find(b =>
                b.ChargingStationId == chargingStationId &&
                b.SlotNumber == slotNumber &&
                b.Status != BookingStatus.Cancelled)
                .ToListAsync();

            // Filter bookings based on their status and time
            var relevantBookings = activeBookings.Where(b =>
            {
                // Include Pending and Confirmed bookings
                if (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                    return true;

                // For Completed bookings, only include if the booking period hasn't ended yet
                if (b.Status == BookingStatus.Completed)
                {
                    var bookingEndTime = b.ReservationDateTime.AddMinutes(b.DurationInMinutes);
                    return bookingEndTime > now; // Include if booking hasn't fully ended
                }

                return false;
            }).ToList();

            // Check if any relevant booking overlaps with the requested time period
            foreach (var booking in relevantBookings)
            {
                var bookingEndTime = booking.ReservationDateTime.AddMinutes(booking.DurationInMinutes);
                
                // Two time periods overlap if:
                // The booking starts before our end time AND the booking ends after our start time
                if (booking.ReservationDateTime < endTime && bookingEndTime > startTime)
                {
                    return false; // Overlap found, slot is not available
                }
            }

            return true; // No overlaps, slot is available
        }

        /// <summary>
        /// Checks slot availability while excluding a specific booking (for update scenarios)
        /// Properly handles Completed bookings by checking if they've ended
        /// </summary>
        public async Task<bool> IsSlotAvailableForDurationAsync(string chargingStationId, int slotNumber, DateTime startTime, int durationInMinutes, string excludeBookingId)
        {
            var endTime = startTime.AddMinutes(durationInMinutes);
            var now = DateTime.UtcNow;

            // Get all bookings for this station and slot, excluding the specified booking and Cancelled bookings
            var activeBookings = await _collection.Find(b =>
                b.ChargingStationId == chargingStationId &&
                b.SlotNumber == slotNumber &&
                b.Id != excludeBookingId &&
                b.Status != BookingStatus.Cancelled)
                .ToListAsync();

            // Filter bookings based on their status and time
            var relevantBookings = activeBookings.Where(b =>
            {
                // Include Pending and Confirmed bookings
                if (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                    return true;

                // For Completed bookings, only include if the booking period hasn't ended yet
                if (b.Status == BookingStatus.Completed)
                {
                    var bookingEndTime = b.ReservationDateTime.AddMinutes(b.DurationInMinutes);
                    return bookingEndTime > now; // Include if booking hasn't fully ended
                }

                return false;
            }).ToList();

            // Check if any relevant booking overlaps with the requested time period
            foreach (var booking in relevantBookings)
            {
                var bookingEndTime = booking.ReservationDateTime.AddMinutes(booking.DurationInMinutes);
                
                // Two time periods overlap if:
                // The booking starts before our end time AND the booking ends after our start time
                if (booking.ReservationDateTime < endTime && bookingEndTime > startTime)
                {
                    return false; // Overlap found, slot is not available
                }
            }

            return true; // No overlaps, slot is available
        }

        // Get all bookings without pagination for dashboard statistics
        public async Task<List<Booking>> GetAllAsync()
        {
            return await _collection.Find(_ => true)
                .SortByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        // New method for admin view of all bookings with pagination and filtering
        public async Task<List<Booking>> GetAllAsync(int page = 1, int pageSize = 50, string? status = null, string? evOwnerNic = null, string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null)
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

            // Apply search term filter (search across multiple fields)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchFilter = Builders<Booking>.Filter.Or(
                    Builders<Booking>.Filter.Regex(b => b.Id, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<Booking>.Filter.Regex(b => b.EvOwnerNic, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<Booking>.Filter.Regex(b => b.ChargingStationId, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                );
                filter &= searchFilter;
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

        // ? New repository methods
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

        // ? Get bookings by station and date range for slot availability checking
        public async Task<List<Booking>> GetBookingsByStationAndDateRangeAsync(string stationId, DateTime startDate, DateTime endDate)
        {
            // Get all bookings (including Completed) that fall within the date range
            // We need to check Completed bookings too because they might still be occupying the slot
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.ChargingStationId, stationId),
                Builders<Booking>.Filter.Gte(b => b.ReservationDateTime, startDate),
                Builders<Booking>.Filter.Lt(b => b.ReservationDateTime, endDate),
                // Only exclude Cancelled bookings - they free up the slot
                Builders<Booking>.Filter.Ne(b => b.Status, BookingStatus.Cancelled)
            );

            var allBookings = await _collection.Find(filter)
                .SortBy(b => b.ReservationDateTime)
                .ToListAsync();

            // Filter out completed bookings that have already ended
            var now = DateTime.UtcNow;
            var activeBookings = allBookings.Where(b =>
            {
                // Include if booking is Pending or Confirmed
                if (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                    return true;

                // For Completed bookings, only include if the booking time hasn't fully passed yet
                if (b.Status == BookingStatus.Completed)
                {
                    var bookingEndTime = b.ReservationDateTime.AddMinutes(b.DurationInMinutes);
                    return bookingEndTime > now; // Only include if booking hasn't ended yet
                }

                return false; // Exclude all other statuses (shouldn't happen due to filter)
            }).ToList();

            return activeBookings;
        }

        // ? Get overlapping bookings for validation
        public async Task<List<Booking>> GetOverlappingBookingsAsync(string stationId, int slotNumber, DateTime startTime, DateTime endTime)
        {
            var now = DateTime.UtcNow;

            // Get all bookings for this station and slot (excluding only Cancelled)
            var bookings = await _collection.Find(b =>
                b.ChargingStationId == stationId &&
                b.SlotNumber == slotNumber &&
                b.Status != BookingStatus.Cancelled)
                .ToListAsync();

            // Filter for actual time overlap, considering booking status and current time
            var overlapping = bookings.Where(b =>
            {
                // For Completed bookings, only include if they haven't ended yet
                if (b.Status == BookingStatus.Completed)
                {
                    var bookingEndTime = b.ReservationDateTime.AddMinutes(b.DurationInMinutes);
                    if (bookingEndTime <= now)
                        return false; // Booking has ended, doesn't block the slot
                }

                // Check for time overlap
                var bookingEnd = b.ReservationDateTime.AddMinutes(b.DurationInMinutes);
                return b.ReservationDateTime < endTime && bookingEnd > startTime;
            }).ToList();

            return overlapping;
        }
    }
}
