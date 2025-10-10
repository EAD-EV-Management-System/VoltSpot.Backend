using Application.DTOs.Response.Dashboard;
using Application.UseCases.Dashboard.Queries;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Dashboard.Handlers
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _stationRepository;
        private readonly IEVOwnerRepository _evOwnerRepository;

        public GetDashboardStatsQueryHandler(
            IBookingRepository bookingRepository,
            IChargingStationRepository stationRepository,
            IEVOwnerRepository evOwnerRepository)
        {
            _bookingRepository = bookingRepository;
            _stationRepository = stationRepository;
            _evOwnerRepository = evOwnerRepository;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            // Get all data in parallel for better performance
            var allBookingsTask = _bookingRepository.GetAllAsync();
            var allStationsTask = _stationRepository.GetAllAsync();
            var allEVOwnersTask = _evOwnerRepository.GetAllAsync();

            await Task.WhenAll(allBookingsTask, allStationsTask, allEVOwnersTask);

            var allBookings = (await allBookingsTask).ToList();
            var allStations = (await allStationsTask).ToList();
            var allEVOwners = (await allEVOwnersTask).ToList();

            // Calculate statistics
            var pendingBookings = allBookings.Count(b => b.Status == Domain.Enums.BookingStatus.Pending);
            var confirmedBookings = allBookings.Count(b => b.Status == Domain.Enums.BookingStatus.Confirmed);
            var completedBookings = allBookings.Count(b => b.Status == Domain.Enums.BookingStatus.Completed);
            
            var activeStations = allStations.Count(s => s.Status == Domain.Enums.StationStatus.Active && !s.IsDeleted);
            var inactiveStations = allStations.Count(s => s.Status != Domain.Enums.StationStatus.Active && !s.IsDeleted);

            // Calculate total revenue (simplified - assuming completed bookings generate revenue)
            var totalRevenue = completedBookings * 25.0m; // Simplified calculation

            // Calculate average utilization
            var averageUtilization = allStations.Where(s => s.TotalSlots > 0)
                .Average(s => (double)(s.TotalSlots - s.AvailableSlots) / s.TotalSlots * 100);

            return new DashboardStatsDto
            {
                TotalBookings = allBookings.Count,
                TotalStations = allStations.Count(s => !s.IsDeleted),
                TotalEVOwners = allEVOwners.Count(),
                TotalRevenue = totalRevenue,
                PendingBookings = pendingBookings,
                ConfirmedBookings = confirmedBookings,
                CompletedBookings = completedBookings,
                ActiveStations = activeStations,
                InactiveStations = inactiveStations,
                AverageUtilization = double.IsNaN(averageUtilization) ? 0 : averageUtilization
            };
        }
    }
}