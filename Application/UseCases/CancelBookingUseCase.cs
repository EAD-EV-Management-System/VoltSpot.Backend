
using System;
using System.Threading.Tasks;
using VoltSpot.Domain.Interfaces;
using VoltSpot.Application.DTOs;

namespace VoltSpot.Application.UseCases
{
    public class CancelBookingUseCase
    {
        private readonly IBookingRepository _bookingRepository;

        public CancelBookingUseCase(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        
        /// Cancels a booking if business rules allow
       
        public async Task<BookingResponseDto> ExecuteAsync(CancelBookingRequestDto request)
        {
            try
            {
                //  Get existing booking
                var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId);
                if (booking == null)
                {
                    return new BookingResponseDto
                    {
                        Success = false,
                        Message = "Booking not found"
                    };
                }

                // Cancel booking (this will check 12-hour rule inside entity)
                booking.Cancel(request.CancellationReason ?? "Cancelled by user");

                //  Save changes
                var cancelledBooking = await _bookingRepository.UpdateBookingAsync(booking);

                return new BookingResponseDto
                {
                    Success = true,
                    Message = "Booking cancelled successfully",
                    BookingId = cancelledBooking.Id,
                    Status = cancelledBooking.Status.ToString()
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