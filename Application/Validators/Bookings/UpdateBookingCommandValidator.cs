using Application.UseCases.Bookings.Commands;
using FluentValidation;

namespace Application.Validators.Bookings
{
    public class UpdateBookingCommandValidator : AbstractValidator<UpdateBookingCommand>
    {
        public UpdateBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty().WithMessage("Booking ID is required");

            RuleFor(x => x.NewReservationDateTime)
                .NotEmpty().WithMessage("New reservation date is required")
                .Must(BeInTheFuture).WithMessage("New reservation must be in the future")
                .Must(BeWithinSevenDays).WithMessage("New reservation must be within 7 days");
        }

        private bool BeInTheFuture(DateTime dateTime)
        {
            return dateTime > DateTime.UtcNow;
        }

        private bool BeWithinSevenDays(DateTime dateTime)
        {
            return dateTime <= DateTime.UtcNow.AddDays(7);
        }
    }
}