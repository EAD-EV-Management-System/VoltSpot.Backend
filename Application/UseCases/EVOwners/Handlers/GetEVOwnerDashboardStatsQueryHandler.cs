using Application.DTOs.Response.EVOwners;
using Application.UseCases.EVOwners.Queries;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.EVOwners.Handlers
{
    public class GetEVOwnerDashboardStatsQueryHandler : IRequestHandler<GetEVOwnerDashboardStatsQuery, EVOwnerDashboardStatsDto>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _stationRepository;

        public GetEVOwnerDashboardStatsQueryHandler(
            IEVOwnerRepository evOwnerRepository,
            IBookingRepository bookingRepository,
            IChargingStationRepository stationRepository)
        {
            _evOwnerRepository = evOwnerRepository;
            _bookingRepository = bookingRepository;
            _stationRepository = stationRepository;
        }

        public async Task<EVOwnerDashboardStatsDto> Handle(GetEVOwnerDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            // 1. Verify EV owner exists
            var evOwner = await _evOwnerRepository.GetByNICAsync(request.EvOwnerNic);
            if (evOwner == null)
            {
                throw new KeyNotFoundException("EV owner not found");
            }

            // 2. Get all bookings for this EV owner
            var allBookings = await _bookingRepository.GetBookingsByEvOwnerAsync(request.EvOwnerNic);

            // 3. Calculate counts
            var now = DateTime.UtcNow;
            var pendingReservations = allBookings.Count(b => b.Status == BookingStatus.Pending);
            var approvedReservations = allBookings.Count(b => 
                b.Status == BookingStatus.Confirmed && 
                b.ReservationDateTime > now
            );
            var completedCharges = allBookings.Count(b => b.Status == BookingStatus.Completed);
            var cancelledBookings = allBookings.Count(b => b.Status == BookingStatus.Cancelled);
            var totalBookings = allBookings.Count;

            // 4. Calculate total spent (assuming there's a cost field in completed bookings)
            // For now, using a default price calculation
            decimal totalSpent = 0;
            foreach (var booking in allBookings.Where(b => b.Status == BookingStatus.Completed))
            {
                // Try to get the station to calculate cost
                var station = await _stationRepository.GetByIdAsync(booking.ChargingStationId);
                if (station != null)
                {
                    // Assuming 2 hours per booking as default duration
                    totalSpent += station.PricePerHour * 2;
                }
            }

            // 5. Get next upcoming booking
            var nextUpcomingBooking = allBookings
                .Where(b => b.Status == BookingStatus.Confirmed && b.ReservationDateTime > now)
                .OrderBy(b => b.ReservationDateTime)
                .FirstOrDefault();

            NextUpcomingBookingDto? nextUpcomingDto = null;
            if (nextUpcomingBooking != null)
            {
                var station = await _stationRepository.GetByIdAsync(nextUpcomingBooking.ChargingStationId);
                nextUpcomingDto = new NextUpcomingBookingDto
                {
                    Id = nextUpcomingBooking.Id,
                    ChargingStationId = nextUpcomingBooking.ChargingStationId,
                    StationName = station?.Name ?? "Unknown Station",
                    ReservationDateTime = nextUpcomingBooking.ReservationDateTime,
                    SlotNumber = nextUpcomingBooking.SlotNumber,
                    Status = nextUpcomingBooking.Status.ToString()
                };
            }

            // 6. Get recent bookings (last 5)
            var recentBookings = new List<RecentBookingDto>();
            foreach (var booking in allBookings.OrderByDescending(b => b.CreatedAt).Take(5))
            {
                var station = await _stationRepository.GetByIdAsync(booking.ChargingStationId);
                recentBookings.Add(new RecentBookingDto
                {
                    Id = booking.Id,
                    ChargingStationId = booking.ChargingStationId,
                    StationName = station?.Name ?? "Unknown Station",
                    ReservationDateTime = booking.ReservationDateTime,
                    Status = booking.Status.ToString(),
                    SlotNumber = booking.SlotNumber,
                    CreatedAt = booking.CreatedAt
                });
            }

            // 7. Return assembled data
            return new EVOwnerDashboardStatsDto
            {
                PendingReservations = pendingReservations,
                ApprovedReservations = approvedReservations,
                CompletedCharges = completedCharges,
                CancelledBookings = cancelledBookings,
                TotalBookings = totalBookings,
                TotalSpent = totalSpent,
                NextUpcomingBooking = nextUpcomingDto,
                RecentBookings = recentBookings
            };
        }
    }
}
