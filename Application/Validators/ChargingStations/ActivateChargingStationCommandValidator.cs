using Application.UseCases.ChargingStations.Commands;
using FluentValidation;

namespace Application.Validators.ChargingStations
{
    public class ActivateChargingStationCommandValidator : AbstractValidator<ActivateChargingStationCommand>
    {
        public ActivateChargingStationCommandValidator()
        {
            RuleFor(x => x.StationId)
                .NotEmpty().WithMessage("Station ID is required")
                .Length(24).WithMessage("Station ID must be a valid MongoDB ObjectId (24 characters)");
        }
    }
}
