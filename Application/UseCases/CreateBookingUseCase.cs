
using System;
using System.Threading.Tasks;
using VoltSpot.Domain.Interfaces;
using VoltSpot.Application.DTOs;
using Domain.Entities;

namespace VoltSpot.Application.UseCases
{
    public class CreateBookingUseCase
    {
        private readonly IBookingRepository _bookingRepository;

        public CreateBookingUseCase(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

       
        /// Creates a new booking following business rules
       
        public async Task<BookingResponseDto> ExecuteAsync(CreateBookingRequestDto request)
        {
            try
            {
                //Check if slot is available
                bool isAvailable = await _bookingRepository.IsSlotAvailableAsync(
                    request.ChargingStationId,
                    request.SlotNumber,
                    request.ReservationDateTime);

                if (!isAvailable)
                {
                    return new BookingResponseDto
                    {
                        Success = false,
                        Message = "Slot is not available at the requested time"
                    };
                }

                //  Create booking with business rules
                var booking = new Booking(
                    request.EvOwnerNic,
                    request.ChargingStationId,
                    request.SlotNumber,
                    request.ReservationDateTime);

                // Save to database
                var savedBooking = await _bookingRepository.CreateBookingAsync(booking);

                //Return success response
                return new BookingResponseDto
                {
                    Success = true,
                    Message = "Booking created successfully",
                    BookingId = savedBooking.Id,
                    QrCode = savedBooking.QrCode,
                    Status = savedBooking.Status.ToString()
                };
            }
            catch (Exception ex)
            {
                return new BookingResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}