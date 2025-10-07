using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using MongoDB.Driver;

namespace Infrastructure.Data.Repositories
{
    public class ChargingStationRepository : BaseRepository<ChargingStation>, IChargingStationRepository
    {        
        public ChargingStationRepository(AppDbContext context) 
            : base(context.ChargingStations)
        {
        }

        public async Task<List<ChargingStation>> GetActiveStationsAsync()
        {
            return await _collection.Find(s => s.Status == StationStatus.Active && !s.IsDeleted)
                                  .ToListAsync();
        }

        public async Task<List<ChargingStation>> GetStationsByLocationAsync(double latitude, double longitude, double radiusKm)
        {
            // Simple distance calculation - for production, consider using MongoDB's geospatial queries
            var allStations = await _collection.Find(s => !s.IsDeleted).ToListAsync();
            
            return allStations.Where(station =>
            {
                var distance = CalculateDistance(latitude, longitude, station.Latitude, station.Longitude);
                return distance <= radiusKm;
            }).ToList();
        }

        public async Task<List<ChargingStation>> GetStationsByTypeAsync(ChargingType type)
        {
            return await _collection.Find(s => s.Type == type && s.Status == StationStatus.Active && !s.IsDeleted)
                                  .ToListAsync();
        }

        public async Task<bool> HasActiveBookingsAsync(string stationId)
        {
            // Check if there are any pending or confirmed bookings for this station
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.ChargingStationId, stationId),
                Builders<Booking>.Filter.In(b => b.Status, new[] { BookingStatus.Pending, BookingStatus.Confirmed })
            );

            // Access the bookings collection directly from the database
            var bookingsCollection = _collection.Database.GetCollection<Booking>("Bookings");
            var activeBooking = await bookingsCollection.Find(filter).FirstOrDefaultAsync();
            
            return activeBooking != null;
        }

        public async Task<List<ChargingStation>> GetStationsByOperatorAsync(string operatorId)
        {
            return await _collection.Find(s => s.AssignedOperatorIds.Contains(operatorId) && !s.IsDeleted)
                                  .ToListAsync();
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}