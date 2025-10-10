using Application.DTOs.Response.Dashboard;
using Application.UseCases.Dashboard.Queries;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Dashboard.Handlers
{
    public class GetStationUtilizationQueryHandler : IRequestHandler<GetStationUtilizationQuery, List<StationUtilizationDto>>
    {
        private readonly IChargingStationRepository _stationRepository;
        private readonly IBookingRepository _bookingRepository;

        public GetStationUtilizationQueryHandler(
            IChargingStationRepository stationRepository,
            IBookingRepository bookingRepository)
        {
            _stationRepository = stationRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<List<StationUtilizationDto>> Handle(GetStationUtilizationQuery request, CancellationToken cancellationToken)
        {
            var stations = await _stationRepository.GetAllAsync();
            var result = new List<StationUtilizationDto>();

            foreach (var station in stations.Where(s => !s.IsDeleted))
            {
                // Get bookings for this station
                var stationBookings = await _bookingRepository.GetBookingsByStationAsync(station.Id);
                var completedBookings = stationBookings.Count(b => b.Status == Domain.Enums.BookingStatus.Completed);
                
                // Calculate revenue (simplified calculation)
                var revenue = completedBookings * station.PricePerHour;

                result.Add(new StationUtilizationDto
                {
                    StationId = station.Id,
                    StationName = station.Name,
                    Location = station.Location,
                    TotalSlots = station.TotalSlots,
                    AvailableSlots = station.AvailableSlots,
                    TotalBookings = stationBookings.Count,
                    Revenue = revenue
                });
            }

            // Sort by utilization percentage descending
            return result.OrderByDescending(s => s.UtilizationPercentage).ToList();
        }
    }
}