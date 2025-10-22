using Application.DTOs.Response.Bookings;
using Application.UseCases.Bookings.Commands;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using VoltSpot.Domain.Interfaces;

namespace Application.UseCases.Bookings.Handlers
{
    public class ValidateBookingCommandHandler : IRequestHandler<ValidateBookingCommand, BookingValidationDto>
    {
        private readonly IChargingStationRepository _stationRepository;
        private readonly IBookingRepository _bookingRepository;

        public ValidateBookingCommandHandler(
            IChargingStationRepository stationRepository,
            IBookingRepository bookingRepository)
        {
            _stationRepository = stationRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingValidationDto> Handle(ValidateBookingCommand request, CancellationToken cancellationToken)
        {
            var result = new BookingValidationDto
            {
                IsValid = true,
                IsAvailable = true,
                CanBook = true,
                ValidationMessages = new List<string>()
            };

            // 1. Check if station exists and is active
            var station = await _stationRepository.GetByIdAsync(request.ChargingStationId);
            if (station == null)
            {
                result.IsValid = false;
                result.CanBook = false;
                result.ValidationMessages.Add("Charging station not found");
                return result;
            }

            if (station.Status != StationStatus.Active)
            {
                result.IsValid = false;
                result.CanBook = false;
                result.ValidationMessages.Add($"Charging station is currently {station.Status}");
                return result;
            }

            // 2. Check if slot number is valid
            if (request.SlotNumber < 1 || request.SlotNumber > station.TotalSlots)
            {
                result.IsValid = false;
                result.CanBook = false;
                result.ValidationMessages.Add($"Invalid slot number. Must be between 1 and {station.TotalSlots}");
                return result;
            }

            // 3. Check if date/time is in the future
            if (request.ReservationDateTime <= DateTime.UtcNow)
            {
                result.IsValid = false;
                result.CanBook = false;
                result.ValidationMessages.Add("Reservation date and time must be in the future");
                return result;
            }

            // 4. Check if reservation is within 7 days
            if (request.ReservationDateTime > DateTime.UtcNow.AddDays(7))
            {
                result.IsValid = false;
                result.CanBook = false;
                result.ValidationMessages.Add("Reservation must be within 7 days from today");
                return result;
            }

            // 5. Check if slot is available at requested time
            var bookingEndTime = request.ReservationDateTime.AddHours(request.Duration);
            var overlappingBookings = await _bookingRepository.GetOverlappingBookingsAsync(
                request.ChargingStationId,
                request.SlotNumber,
                request.ReservationDateTime,
                bookingEndTime
            );

            if (overlappingBookings.Any())
            {
                result.IsAvailable = false;
                result.CanBook = false;
                var firstBooking = overlappingBookings.First();
                result.ValidationMessages.Add(
                    $"Slot {request.SlotNumber} is already booked from " +
                    $"{firstBooking.ReservationDateTime:HH:mm} to " +
                    $"{firstBooking.ReservationDateTime.AddHours(2):HH:mm}"
                );
                result.ValidationMessages.Add("Please select a different slot or time");

                // Generate suggestions
                result.Suggestions = await GenerateSuggestionsAsync(request, station);
            }

            // 6. Check if station is open during requested time
            if (!IsStationOpen(station, request.ReservationDateTime))
            {
                result.IsValid = false;
                result.CanBook = false;
                result.ValidationMessages.Add("Station is closed at the requested time");

                var openTime = station.OperatingHours?.OpenTime ?? TimeSpan.Zero;
                var closeTime = station.OperatingHours?.CloseTime ?? new TimeSpan(23, 59, 59);
                result.ValidationMessages.Add($"Station operating hours: {openTime:hh\\:mm} - {closeTime:hh\\:mm}");
            }

            // 7. Calculate estimated cost
            if (result.CanBook)
            {
                result.EstimatedCost = station.PricePerHour * request.Duration;
            }

            return result;
        }

        private async Task<BookingSuggestionsDto> GenerateSuggestionsAsync(ValidateBookingCommand request, Domain.Entities.ChargingStation station)
        {
            var suggestions = new BookingSuggestionsDto();

            // Get all bookings for this station on the requested date
            var startOfDay = request.ReservationDateTime.Date;
            var endOfDay = startOfDay.AddDays(1);
            var bookingsOnDate = await _bookingRepository.GetBookingsByStationAndDateRangeAsync(
                request.ChargingStationId,
                startOfDay,
                endOfDay
            );

            // Find alternative slots
            var bookedSlots = bookingsOnDate
                .Where(b =>
                {
                    var bookingEnd = b.ReservationDateTime.AddHours(2);
                    return b.ReservationDateTime < request.ReservationDateTime.AddHours(request.Duration) &&
                           bookingEnd > request.ReservationDateTime;
                })
                .Select(b => b.SlotNumber)
                .Distinct()
                .ToHashSet();

            suggestions.AlternativeSlots = Enumerable.Range(1, station.TotalSlots)
                .Except(bookedSlots)
                .ToList();

            // Find alternative times for the same slot (next 3 hours in 1-hour increments)
            for (int i = 1; i <= 3; i++)
            {
                var alternativeTime = request.ReservationDateTime.AddHours(i);
                if (alternativeTime.Date != request.ReservationDateTime.Date)
                    break; // Stay within the same day

                var alternativeEnd = alternativeTime.AddHours(request.Duration);
                var hasConflict = bookingsOnDate.Any(b =>
                    b.SlotNumber == request.SlotNumber &&
                    b.ReservationDateTime < alternativeEnd &&
                    b.ReservationDateTime.AddHours(2) > alternativeTime
                );

                suggestions.AlternativeTimes.Add(new AlternativeTimeDto
                {
                    Time = alternativeTime.ToString("HH:mm"),
                    SlotNumber = request.SlotNumber,
                    Available = !hasConflict
                });
            }

            // Note: Nearby stations would require geolocation logic
            // For now, leaving it empty
            suggestions.NearbyStations = new List<NearbyStationDto>();

            return suggestions;
        }

        private bool IsStationOpen(Domain.Entities.ChargingStation station, DateTime requestedTime)
        {
            if (station.OperatingHours == null)
                return true; // Assume 24/7 if no hours specified

            if (station.OperatingHours.Is24Hours && 
                !station.OperatingHours.ClosedDays.Contains(requestedTime.DayOfWeek))
                return true;

            if (station.OperatingHours.ClosedDays.Contains(requestedTime.DayOfWeek))
                return false;

            var requestedTimeOfDay = requestedTime.TimeOfDay;
            return requestedTimeOfDay >= station.OperatingHours.OpenTime && 
                   requestedTimeOfDay <= station.OperatingHours.CloseTime;
        }
    }
}
