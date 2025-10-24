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
    //[Authorize]
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
        /// Search charging stations with advanced filtering
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchChargingStations(
            [FromQuery] string? location = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] ChargingType? type = null,
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null,
            [FromQuery] double? radiusKm = 10,
            [FromQuery] bool? availableOnly = true,
            [FromQuery] decimal? maxPricePerHour = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = new SearchChargingStationsQuery
            {
                Location = location ?? searchTerm, // Use searchTerm as location if location not provided
                Type = type,
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                AvailableOnly = availableOnly,
                MaxPricePerHour = maxPricePerHour,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Success(result);
        }

        /// <summary>
        /// Get nearby charging stations based on location
        /// </summary>
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyStations(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 10,
            [FromQuery] bool availableOnly = true,
            [FromQuery] int limit = 20)
        {
            var query = new SearchChargingStationsQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                AvailableOnly = availableOnly,
                Page = 1,
                PageSize = limit
            };

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
        /// Get available slots for a charging station on a specific date/time
        /// </summary>
        [HttpGet("{id}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(
            string id,
            [FromQuery] string date,
            [FromQuery] string? time = null,
            [FromQuery] int durationInMinutes = 120)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return Error("Invalid station ID format");
                }

                if (!DateTime.TryParse(date, out var parsedDate))
                {
                    return Error("Invalid date format. Please use YYYY-MM-DD format");
                }

                if (durationInMinutes <= 0 || durationInMinutes > 480)
                {
                    return Error("Duration must be between 1 and 480 minutes");
                }

                TimeSpan? parsedTime = null;
                if (!string.IsNullOrEmpty(time))
                {
                    if (!TimeSpan.TryParse(time, out var timeSpan))
                    {
                        return Error("Invalid time format. Please use HH:mm format");
                    }
                    parsedTime = timeSpan;
                }

                var query = new GetAvailableSlotsQuery
                {
                    StationId = id,
                    Date = parsedDate,
                    Time = parsedTime,
                    DurationInMinutes = durationInMinutes
                };

                var result = await _mediator.Send(query);
                return Success(result, "Available slots retrieved successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Error("An internal server error occurred", 500);
            }
        }

        /// <summary>
        /// Update slot configuration (Station Operator or Backoffice)
        /// WARNING: This endpoint is deprecated for updating AvailableSlots.
        /// Use GET /api/chargingstations/{id}/available-slots to check time-based availability.
        /// This should only be used to update TotalSlots (physical capacity of the station).
        /// </summary>
        [HttpPatch("{id}/slots")]
        [Authorize(Roles = "Backoffice,StationOperator")]
        [Obsolete("Updating AvailableSlots manually is deprecated. Use GET /api/chargingstations/{id}/available-slots for time-based availability.")]
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
                
                // Return success with deprecation warning
                return Success(new
                {
                    message = "Slot configuration updated successfully",
                    warning = "WARNING: Manually updating AvailableSlots is deprecated. " +
                             "Slot availability is time-based and should be queried using " +
                             "GET /api/chargingstations/{id}/available-slots with date and time parameters."
                });
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

        /// <summary>
        /// Activate charging station (Backoffice only)
        /// </summary>
        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Backoffice")]
        public async Task<IActionResult> ActivateChargingStation(string id, [FromBody] ActivateChargingStationRequestDto? request = null)
        {
            try
            {
                // Validate ObjectId format
                if (!ObjectId.TryParse(id, out _))
                {
                    return Error("Invalid station ID format");
                }

                var command = new ActivateChargingStationCommand 
                { 
                    StationId = id,
                    ActivatedBy = request?.ActivatedBy,
                    Notes = request?.Notes
                };
                
                var result = await _mediator.Send(command);
                return Success("Charging station activated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Error($"An error occurred: {ex.Message}", 500);
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