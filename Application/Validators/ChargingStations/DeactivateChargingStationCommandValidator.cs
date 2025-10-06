using Application.UseCases.ChargingStations.Commands;
using FluentValidation;

namespace Application.Validators.ChargingStations
{
    public class DeactivateChargingStationCommandValidator : AbstractValidator<DeactivateChargingStationCommand>
    {
        public DeactivateChargingStationCommandValidator()
        {
            RuleFor(x => x.StationId)
                .NotEmpty().WithMessage("Station ID is required");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Reason));

            RuleFor(x => x.DeactivatedBy)
                .MaximumLength(100).WithMessage("Deactivated by must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.DeactivatedBy));

            RuleFor(x => x.EstimatedReactivationDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Estimated reactivation date must be in the future")
                .When(x => x.EstimatedReactivationDate.HasValue);
        }
    }
}