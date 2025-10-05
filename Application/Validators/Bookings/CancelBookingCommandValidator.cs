using Application.UseCases.Bookings.Commands;
using FluentValidation;

namespace Application.Validators.Bookings
{
    public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
    {
        public CancelBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty().WithMessage("Booking ID is required");

            RuleFor(x => x.CancellationReason)
                .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters");
        }
    }
}
