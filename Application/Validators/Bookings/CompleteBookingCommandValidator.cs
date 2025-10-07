
using Application.UseCases.Bookings.Commands;
using FluentValidation;

namespace Application.Validators.Bookings
{
    public class CompleteBookingCommandValidator : AbstractValidator<CompleteBookingCommand>
    {
        public CompleteBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty().WithMessage("Booking ID is required");
        }
    }
}