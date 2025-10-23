using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces.Repositories
{
    public interface IChargingStationRepository
    {
        Task<ChargingStation?> GetByIdAsync(string id);
        Task<ChargingStation> AddAsync(ChargingStation station);
        Task<ChargingStation> UpdateAsync(ChargingStation station);
        Task DeleteAsync(string id);
        Task<IEnumerable<ChargingStation>> GetAllAsync();
        Task<List<ChargingStation>> GetActiveStationsAsync();
        Task<List<ChargingStation>> GetStationsByLocationAsync(double latitude, double longitude, double radiusKm);
        Task<List<ChargingStation>> GetStationsByTypeAsync(ChargingType type);
        Task<bool> HasActiveBookingsAsync(string stationId);
        Task<bool> ExistsAsync(string id);
        Task<List<ChargingStation>> GetStationsByOperatorAsync(string operatorId);
        Task<List<ChargingStation>> SearchAsync(string? location = null, ChargingType? type = null, double? latitude = null, double? longitude = null, double? radiusKm = null, bool? availableOnly = null, decimal? maxPricePerHour = null, int page = 1, int pageSize = 20);
    }
}