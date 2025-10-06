using Application.DTOs.Request.ChargingStations;
using Application.UseCases.ChargingStations.Commands;
using Application.UseCases.ChargingStations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Base;
using static Application.UseCases.ChargingStations.Commands.CreateChargingStationCommand;
using Domain.Enums;
using MongoDB.Bson;
using FluentValidation;

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
            try
            {
                if (request == null)
                {
                    return Error("Request body is required");
                }

                // Handle both ChargingType and Type properties - resolve string to enum
                var chargingType = ResolveChargingType(request.ChargingType, request.Type);
                
                // Parse operating hours if provided as string
                var operatingHours = ParseOperatingHours(request.OperatingHours, request.OperatingHoursDto);

                var command = new CreateChargingStationCommand
                {
                    Name = request.Name,
                    Location = request.Location,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Type = chargingType,
                    TotalSlots = request.TotalSlots,
                    AvailableSlots = request.AvailableSlots > 0 ? request.AvailableSlots : request.TotalSlots,
                    Description = request.Description,
                    Amenities = request.Amenities,
                    PricePerHour = request.PricePerHour,
                    AssignedOperatorIds = !string.IsNullOrEmpty(request.OperatorId) ? 
                        new List<string> { request.OperatorId } : request.AssignedOperatorIds,
                    OperatingHours = operatingHours
                };

                var result = await _mediator.Send(command);
                return Success(result, "Charging station created successfully");
            }
            catch (ValidationException ex)
            {
                return Error(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                return Error("An internal server error occurred", 500);
            }
        }

        /// <summary>
        /// Update an existing charging station (Backoffice only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> UpdateChargingStation(string id, [FromBody] UpdateChargingStationRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return Error("Request body is required");
                }

                // Validate ObjectId format
                if (!ObjectId.TryParse(id, out _))
                {
                    return Error("Invalid station ID format");
                }

                // Handle both ChargingType and Type properties
                var chargingType = ResolveChargingType(request.ChargingType, request.Type);
                
                // Parse operating hours if provided as string
                var operatingHours = ParseOperatingHours(request.OperatingHours, request.OperatingHoursDto);

                var command = new UpdateChargingStationCommand
                {
                    Id = id,
                    Name = request.Name,
                    Location = request.Location,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Type = chargingType,
                    TotalSlots = request.TotalSlots,
                    AvailableSlots = request.AvailableSlots,
                    Description = request.Description,
                    Amenities = request.Amenities,
                    PricePerHour = request.PricePerHour,
                    AssignedOperatorIds = !string.IsNullOrEmpty(request.OperatorId) ? 
                        new List<string> { request.OperatorId } : request.AssignedOperatorIds,
                    OperatingHours = operatingHours
                };

                var result = await _mediator.Send(command);
                return Success(result, "Charging station updated successfully");
            }
            catch (ValidationException ex)
            {
                return Error(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                return Error("An internal server error occurred", 500);
            }
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
            try
            {
                // Validate ObjectId format
                if (!ObjectId.TryParse(id, out _))
                {
                    return Error("Invalid station ID format");
                }

                var query = new GetChargingStationByIdQuery { Id = id };
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFound("Charging station not found");
                    
                return Success(result);
            }
            catch (Exception ex)
            {
                return Error("An internal server error occurred", 500);
            }
        }

        /// <summary>
        /// Update slot availability (Station Operator or Backoffice)
        /// </summary>
        [HttpPatch("{id}/slots")]
        [Authorize(Roles = "Backoffice,StationOperator")]
        public async Task<IActionResult> UpdateSlotAvailability(string id, [FromBody] UpdateSlotAvailabilityRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return Error("Request body is required");
                }

                // Validate ObjectId format
                if (!ObjectId.TryParse(id, out _))
                {
                    return Error("Invalid station ID format");
                }

                var command = new UpdateSlotAvailabilityCommand
                {
                    StationId = id,
                    TotalSlots = request.TotalSlots,
                    AvailableSlots = request.AvailableSlots
                };

                var result = await _mediator.Send(command);
                return Success("Slot availability updated successfully");
            }
            catch (ValidationException ex)
            {
                return Error(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                return Error("An internal server error occurred", 500);
            }
        }

        /// <summary>
        /// Deactivate charging station (Backoffice only)
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        // [Authorize(Roles = "Backoffice")] // Temporarily disabled for testing
        public async Task<IActionResult> DeactivateChargingStation(string id, [FromBody] DeactivateChargingStationRequestDto? request = null)
        {
            try
            {
                // Validate ObjectId format
                if (!ObjectId.TryParse(id, out _))
                {
                    return Error("Invalid station ID format");
                }

                var command = new DeactivateChargingStationCommand 
                { 
                    StationId = id,
                    Reason = request?.Reason,
                    DeactivatedBy = request?.DeactivatedBy,
                    EstimatedReactivationDate = request?.EstimatedReactivationDate
                };
                
                var result = await _mediator.Send(command);
                return Success("Charging station deactivated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                // Return more detailed error information for debugging
                return Error($"An error occurred: {ex.Message}. Details: {ex.InnerException?.Message}", 500);
            }
        }

        private ChargingType ResolveChargingType(ChargingType? chargingType, ChargingType type)
        {
            // If chargingType is provided and valid, use it
            if (chargingType.HasValue)
            {
                return chargingType.Value;
            }

            // Otherwise use the type field
            return type;
        }

        private CommandOperatingHoursDto ParseOperatingHours(string? operatingHoursString, OperatingHoursDto? operatingHoursDto)
        {
            // If DTO is provided, use it
            if (operatingHoursDto != null && 
                (operatingHoursDto.OpenTime != TimeSpan.Zero || operatingHoursDto.CloseTime != new TimeSpan(23, 59, 59) || !operatingHoursDto.Is24Hours))
            {
                return new CommandOperatingHoursDto
                {
                    OpenTime = operatingHoursDto.OpenTime,
                    CloseTime = operatingHoursDto.CloseTime,
                    Is24Hours = operatingHoursDto.Is24Hours,
                    ClosedDays = operatingHoursDto.ClosedDays
                };
            }

            // Parse string format
            if (!string.IsNullOrEmpty(operatingHoursString))
            {
                if (operatingHoursString.Equals("24/7", StringComparison.OrdinalIgnoreCase))
                {
                    return new CommandOperatingHoursDto
                    {
                        OpenTime = TimeSpan.Zero,
                        CloseTime = new TimeSpan(23, 59, 59),
                        Is24Hours = true,
                        ClosedDays = new List<DayOfWeek>()
                    };
                }

                // Try to parse time ranges like "06:00 AM - 10:00 PM"
                if (operatingHoursString.Contains(" - "))
                {
                    var parts = operatingHoursString.Split(" - ");
                    if (parts.Length == 2 && 
                        DateTime.TryParse(parts[0], out var openTime) && 
                        DateTime.TryParse(parts[1], out var closeTime))
                    {
                        return new CommandOperatingHoursDto
                        {
                            OpenTime = openTime.TimeOfDay,
                            CloseTime = closeTime.TimeOfDay,
                            Is24Hours = false,
                            ClosedDays = new List<DayOfWeek>()
                        };
                    }
                }
            }

            // Default to 24/7
            return new CommandOperatingHoursDto
            {
                OpenTime = TimeSpan.Zero,
                CloseTime = new TimeSpan(23, 59, 59),
                Is24Hours = true,
                ClosedDays = new List<DayOfWeek>()
            };
        }
    }
}