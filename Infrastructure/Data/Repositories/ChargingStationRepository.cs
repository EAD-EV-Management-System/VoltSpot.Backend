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
        private readonly AppDbContext _context;

        public ChargingStationRepository(AppDbContext context) 
            : base(context.ChargingStations)
        {
            _context = context;
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

            // Access the bookings collection from the context
            var activeBooking = await _context.Bookings.Find(filter).FirstOrDefaultAsync();
            
            return activeBooking != null;
        }

        public async Task<List<ChargingStation>> GetStationsByOperatorAsync(string operatorId)
        {
            return await _collection.Find(s => s.AssignedOperatorIds.Contains(operatorId) && !s.IsDeleted)
                                  .ToListAsync();
        }

        // New comprehensive search method
        public async Task<List<ChargingStation>> SearchAsync(
            string? location = null,
            ChargingType? type = null,
            double? latitude = null,
            double? longitude = null,
            double? radiusKm = null,
            bool? availableOnly = null,
            decimal? maxPricePerHour = null,
            int page = 1,
            int pageSize = 20)
        {
            var filter = Builders<ChargingStation>.Filter.Eq(s => s.IsDeleted, false);

            // Apply status filter (only active stations)
            filter &= Builders<ChargingStation>.Filter.Eq(s => s.Status, StationStatus.Active);

            // Apply type filter
            if (type.HasValue)
            {
                filter &= Builders<ChargingStation>.Filter.Eq(s => s.Type, type.Value);
            }

            // Apply location text search
            if (!string.IsNullOrEmpty(location))
            {
                var locationFilter = Builders<ChargingStation>.Filter.Or(
                    Builders<ChargingStation>.Filter.Regex(s => s.Name, new MongoDB.Bson.BsonRegularExpression(location, "i")),
                    Builders<ChargingStation>.Filter.Regex(s => s.Location, new MongoDB.Bson.BsonRegularExpression(location, "i"))
                );
                filter &= locationFilter;
            }

            // Apply availability filter
            if (availableOnly == true)
            {
                filter &= Builders<ChargingStation>.Filter.Gt(s => s.AvailableSlots, 0);
            }

            // Apply price filter
            if (maxPricePerHour.HasValue)
            {
                filter &= Builders<ChargingStation>.Filter.Lte(s => s.PricePerHour, maxPricePerHour.Value);
            }

            var stations = await _collection.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Apply geographical filtering if coordinates are provided
            if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
            {
                stations = stations.Where(station =>
                {
                    var distance = CalculateDistance(latitude.Value, longitude.Value, station.Latitude, station.Longitude);
                    return distance <= radiusKm.Value;
                }).ToList();
            }

            return stations;
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