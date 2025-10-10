using Application.DTOs.Response.Dashboard;
using Application.UseCases.Dashboard.Queries;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Dashboard.Handlers
{
    public class GetRecentBookingsQueryHandler : IRequestHandler<GetRecentBookingsQuery, List<RecentBookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IChargingStationRepository _stationRepository;
        private readonly IEVOwnerRepository _evOwnerRepository;

        public GetRecentBookingsQueryHandler(
            IBookingRepository bookingRepository,
            IChargingStationRepository stationRepository,
            IEVOwnerRepository evOwnerRepository)
        {
            _bookingRepository = bookingRepository;
            _stationRepository = stationRepository;
            _evOwnerRepository = evOwnerRepository;
        }

        public async Task<List<RecentBookingDto>> Handle(GetRecentBookingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get recent bookings
                var recentBookings = await _bookingRepository.GetRecentBookingsAsync(request.Count);
                
                var result = new List<RecentBookingDto>();

                // If no bookings exist, return empty list
                if (recentBookings == null || !recentBookings.Any())
                {
                    return result;
                }

                foreach (var booking in recentBookings)
                {
                    try
                    {
                        // Get station and owner details with null safety
                        var station = await _stationRepository.GetByIdAsync(booking.ChargingStationId);
                        var evOwner = await _evOwnerRepository.GetByNICAsync(booking.EvOwnerNic);

                        result.Add(new RecentBookingDto
                        {
                            Id = booking.Id,
                            EvOwnerNic = booking.EvOwnerNic ?? "Unknown",
                            EvOwnerName = evOwner != null ? $"{evOwner.FirstName} {evOwner.LastName}".Trim() : "Unknown",
                            ChargingStationId = booking.ChargingStationId ?? "Unknown",
                            ChargingStationName = station?.Name ?? "Unknown Station",
                            ReservationDateTime = booking.ReservationDateTime,
                            Status = booking.Status.ToString(),
                            CreatedAt = booking.CreatedAt,
                            SlotNumber = booking.SlotNumber
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log individual booking error but continue processing
                        // Add a booking entry with error info for debugging
                        result.Add(new RecentBookingDto
                        {
                            Id = booking.Id,
                            EvOwnerNic = booking.EvOwnerNic ?? "Unknown",
                            EvOwnerName = $"Error: {ex.Message}",
                            ChargingStationId = booking.ChargingStationId ?? "Unknown",
                            ChargingStationName = "Error loading station",
                            ReservationDateTime = booking.ReservationDateTime,
                            Status = booking.Status.ToString(),
                            CreatedAt = booking.CreatedAt,
                            SlotNumber = booking.SlotNumber
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // Re-throw with more specific error message
                throw new InvalidOperationException($"Error retrieving recent bookings: {ex.Message}", ex);
            }
        }
    }
}