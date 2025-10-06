using Application.UseCases.ChargingStations.Commands;
using FluentValidation;

namespace Application.Validators.ChargingStations
{
    public class CreateChargingStationCommandValidator : AbstractValidator<CreateChargingStationCommand>
    {
        public CreateChargingStationCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Station name is required")
                .MaximumLength(100).WithMessage("Station name must not exceed 100 characters");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid charging type");

            RuleFor(x => x.TotalSlots)
                .GreaterThan(0).WithMessage("Total slots must be greater than 0")
                .LessThanOrEqualTo(50).WithMessage("Total slots cannot exceed 50");

            RuleFor(x => x.AvailableSlots)
                .GreaterThanOrEqualTo(0).WithMessage("Available slots cannot be negative")
                .LessThanOrEqualTo(x => x.TotalSlots).WithMessage("Available slots cannot exceed total slots");

            RuleFor(x => x.PricePerHour)
                .GreaterThanOrEqualTo(0).WithMessage("Price per hour cannot be negative");

            RuleFor(x => x.OperatingHours)
                .NotNull().WithMessage("Operating hours are required");

            When(x => x.OperatingHours != null && !x.OperatingHours.Is24Hours, () =>
            {
                RuleFor(x => x.OperatingHours.OpenTime)
                    .LessThan(x => x.OperatingHours.CloseTime)
                    .WithMessage("Open time must be before close time");
            });
        }
    }
}