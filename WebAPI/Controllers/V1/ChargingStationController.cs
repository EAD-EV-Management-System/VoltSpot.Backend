using Application.DTOs.Request.ChargingStations;
using Application.UseCases.ChargingStations.Commands;
using Application.UseCases.ChargingStations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Base;
using static Application.UseCases.ChargingStations.Commands.CreateChargingStationCommand;

namespace WebAPI.Controllers.V1
{
    [Authorize]
    public class ChargingStationController : BaseController
    {
        private readonly IMediator _mediator;

        public ChargingStationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new charging station (Backoffice only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> CreateChargingStation([FromBody] CreateChargingStationRequestDto request)
        {
            var command = new CreateChargingStationCommand
            {
                Name = request.Name,
                Location = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Type = request.Type,
                TotalSlots = request.TotalSlots,
                AvailableSlots = request.AvailableSlots,
                Description = request.Description,
                Amenities = request.Amenities,
                PricePerHour = request.PricePerHour,
                AssignedOperatorIds = request.AssignedOperatorIds,
                OperatingHours = new CommandOperatingHoursDto
                {
                    OpenTime = request.OperatingHours.OpenTime,
                    CloseTime = request.OperatingHours.CloseTime,
                    Is24Hours = request.OperatingHours.Is24Hours,
                    ClosedDays = request.OperatingHours.ClosedDays
                }
            };

            var result = await _mediator.Send(command);
            return Success(result, "Charging station created successfully");
        }

        /// <summary>
        /// Update an existing charging station (Backoffice only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> UpdateChargingStation(string id, [FromBody] UpdateChargingStationRequestDto request)
        {
            var command = new UpdateChargingStationCommand
            {
                Id = id,
                Name = request.Name,
                Location = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Type = request.Type,
                TotalSlots = request.TotalSlots,
                AvailableSlots = request.AvailableSlots,
                Description = request.Description,
                Amenities = request.Amenities,
                PricePerHour = request.PricePerHour,
                AssignedOperatorIds = request.AssignedOperatorIds,
                OperatingHours = new CommandOperatingHoursDto
                {
                    OpenTime = request.OperatingHours.OpenTime,
                    CloseTime = request.OperatingHours.CloseTime,
                    Is24Hours = request.OperatingHours.Is24Hours,
                    ClosedDays = request.OperatingHours.ClosedDays
                }
            };

            var result = await _mediator.Send(command);
            return Success(result, "Charging station updated successfully");
        }

        /// <summary>
        /// Get all charging stations
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllChargingStations([FromQuery] bool onlyActive = false)
        {
            var query = new GetAllChargingStationsQuery { OnlyActive = onlyActive };
            var result = await _mediator.Send(query);
            return Success(result);
        }

        /// <summary>
        /// Get charging station by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChargingStationById(string id)
        {
            var query = new GetChargingStationByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound("Charging station not found");
                
            return Success(result);
        }

        /// <summary>
        /// Update slot availability (Station Operator or Backoffice)
        /// </summary>
        [HttpPatch("{id}/slots")]
        [Authorize(Roles = "Backoffice,StationOperator")]
        public async Task<IActionResult> UpdateSlotAvailability(string id, [FromBody] UpdateSlotAvailabilityRequestDto request)
        {
            var command = new UpdateSlotAvailabilityCommand
            {
                StationId = id,
                AvailableSlots = request.AvailableSlots
            };

            var result = await _mediator.Send(command);
            return Success("Slot availability updated successfully");
        }

        /// <summary>
        /// Deactivate charging station (Backoffice only)
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> DeactivateChargingStation(string id)
        {
            var command = new DeactivateChargingStationCommand { StationId = id };
            
            try
            {
                var result = await _mediator.Send(command);
                return Success("Charging station deactivated successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}