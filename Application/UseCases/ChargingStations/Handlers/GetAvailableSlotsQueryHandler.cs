using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Queries;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, AvailableSlotsDto>
    {
        private readonly IChargingStationRepository _stationRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEVOwnerRepository _evOwnerRepository;

        public GetAvailableSlotsQueryHandler(
            IChargingStationRepository stationRepository,
            IBookingRepository bookingRepository,
            IEVOwnerRepository evOwnerRepository)
        {
            _stationRepository = stationRepository;
            _bookingRepository = bookingRepository;
            _evOwnerRepository = evOwnerRepository;
        }

        public async Task<AvailableSlotsDto> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            // 1. Verify station exists
            var station = await _stationRepository.GetByIdAsync(request.StationId);
            if (station == null)
            {
                throw new KeyNotFoundException("Charging station not found");
            }

            // 2. Get all bookings for this station on the specified date
            var startOfDay = request.Date.Date;
            var endOfDay = request.Date.Date.AddDays(1);

            var bookingsOnDate = await _bookingRepository.GetBookingsByStationAndDateRangeAsync(
                request.StationId,
                startOfDay,
                endOfDay
            );

            // 3. Determine booked slots
            HashSet<int> bookedSlotNumbers;
            List<BookedSlotDto> bookedSlotDetails = new();

            if (request.Time.HasValue)
            {
                // Check availability at specific time
                var requestedDateTime = request.Date.Date.Add(request.Time.Value);
                var requestedEndTime = requestedDateTime.AddHours(2); // Assuming 2-hour duration

                foreach (var booking in bookingsOnDate)
                {
                    var bookingEndTime = booking.ReservationDateTime.AddHours(2);
                    
                    // Check if times overlap
                    if (booking.ReservationDateTime < requestedEndTime && bookingEndTime > requestedDateTime)
                    {
                        var evOwner = await _evOwnerRepository.GetByNICAsync(booking.EvOwnerNic);
                        var maskedName = MaskName(evOwner?.FirstName + " " + evOwner?.LastName);

                        bookedSlotDetails.Add(new BookedSlotDto
                        {
                            SlotNumber = booking.SlotNumber,
                            BookedBy = maskedName,
                            BookedFrom = booking.ReservationDateTime,
                            BookedUntil = bookingEndTime,
                            BookingId = booking.Id
                        });
                    }
                }

                bookedSlotNumbers = bookedSlotDetails.Select(b => b.SlotNumber).ToHashSet();
            }
            else
            {
                // Check all-day availability (any booking on that slot makes it unavailable)
                foreach (var booking in bookingsOnDate)
                {
                    var evOwner = await _evOwnerRepository.GetByNICAsync(booking.EvOwnerNic);
                    var maskedName = MaskName(evOwner?.FirstName + " " + evOwner?.LastName);
                    var bookingEndTime = booking.ReservationDateTime.AddHours(2);

                    bookedSlotDetails.Add(new BookedSlotDto
                    {
                        SlotNumber = booking.SlotNumber,
                        BookedBy = maskedName,
                        BookedFrom = booking.ReservationDateTime,
                        BookedUntil = bookingEndTime,
                        BookingId = booking.Id
                    });
                }

                bookedSlotNumbers = bookedSlotDetails.Select(b => b.SlotNumber).Distinct().ToHashSet();
            }

            // 4. Calculate available slots
            var allSlots = Enumerable.Range(1, station.TotalSlots).ToList();
            var availableSlots = allSlots.Except(bookedSlotNumbers).ToList();

            // 5. Get maintenance slots (if applicable - for now returning empty list)
            var maintenanceSlots = new List<int>();
            availableSlots = availableSlots.Except(maintenanceSlots).ToList();

            // 6. Return assembled data
            return new AvailableSlotsDto
            {
                StationId = request.StationId,
                StationName = station.Name,
                Date = request.Date.ToString("yyyy-MM-dd"),
                TotalSlots = station.TotalSlots,
                AvailableSlots = availableSlots,
                AvailableCount = availableSlots.Count,
                BookedSlots = bookedSlotDetails,
                MaintenanceSlots = maintenanceSlots,
                IsFullyBooked = availableSlots.Count == 0
            };
        }

        private string MaskName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "Unknown";

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return fullName;

            return $"{parts[0]} {parts[1][0]}.";
        }
    }
}
