using Application.UseCases.Bookings.Commands;
using FluentValidation;

namespace Application.Validators.Bookings
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.EvOwnerNic)
                .NotEmpty().WithMessage("EV Owner NIC is required")
                .MaximumLength(20).WithMessage("EV Owner NIC must not exceed 20 characters");

            RuleFor(x => x.ChargingStationId)
                .NotEmpty().WithMessage("Charging Station ID is required");

            RuleFor(x => x.SlotNumber)
                .GreaterThan(0).WithMessage("Slot number must be greater than 0");

            RuleFor(x => x.ReservationDateTime)
                .NotEmpty().WithMessage("Reservation date and time is required")
                .Must(BeInTheFuture).WithMessage("Reservation date must be in the future")
                .Must(BeWithinSevenDays).WithMessage("Reservation date must be within 7 days from today");
        }

        private bool BeInTheFuture(DateTime reservationDateTime)
        {
            return reservationDateTime > DateTime.UtcNow;
        }

        private bool BeWithinSevenDays(DateTime reservationDateTime)
        {
            return reservationDateTime <= DateTime.UtcNow.AddDays(7);
        }
    }
}
