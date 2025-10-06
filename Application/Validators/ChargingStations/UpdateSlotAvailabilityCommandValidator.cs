using Application.UseCases.ChargingStations.Commands;
using FluentValidation;

namespace Application.Validators.ChargingStations
{
    public class UpdateSlotAvailabilityCommandValidator : AbstractValidator<UpdateSlotAvailabilityCommand>
    {
        public UpdateSlotAvailabilityCommandValidator()
        {
            RuleFor(x => x.StationId)
                .NotEmpty().WithMessage("Station ID is required");

            RuleFor(x => x.TotalSlots)
                .GreaterThan(0).WithMessage("Total slots must be greater than 0");

            RuleFor(x => x.AvailableSlots)
                .GreaterThanOrEqualTo(0).WithMessage("Available slots cannot be negative")
                .LessThanOrEqualTo(x => x.TotalSlots).WithMessage("Available slots must be between 0 and total slots");
        }
    }
}