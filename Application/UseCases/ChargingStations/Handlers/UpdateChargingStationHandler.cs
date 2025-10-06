using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class UpdateChargingStationHandler : IRequestHandler<UpdateChargingStationCommand, ChargingStationResponseDto>
    {
        private readonly IChargingStationRepository _repository;
        private readonly IMapper _mapper;

        public UpdateChargingStationHandler(IChargingStationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ChargingStationResponseDto> Handle(UpdateChargingStationCommand request, CancellationToken cancellationToken)
        {
            var existingStation = await _repository.GetByIdAsync(request.Id);
            if (existingStation == null)
                throw new KeyNotFoundException($"Charging station with ID {request.Id} not found");

            // Update properties
            existingStation.Name = request.Name;
            existingStation.Location = request.Location;
            existingStation.Latitude = request.Latitude;
            existingStation.Longitude = request.Longitude;
            existingStation.Type = request.Type;
            existingStation.TotalSlots = request.TotalSlots;
            existingStation.AvailableSlots = request.AvailableSlots;
            existingStation.Description = request.Description;
            existingStation.Amenities = request.Amenities;
            existingStation.PricePerHour = request.PricePerHour;
            existingStation.AssignedOperatorIds = request.AssignedOperatorIds;
            
            existingStation.OperatingHours = new OperatingHours
            {
                OpenTime = request.OperatingHours.OpenTime,
                CloseTime = request.OperatingHours.CloseTime,
                Is24Hours = request.OperatingHours.Is24Hours,
                ClosedDays = request.OperatingHours.ClosedDays
            };

            existingStation.UpdatedAt = DateTime.UtcNow;

            var updatedStation = await _repository.UpdateAsync(existingStation);
            return _mapper.Map<ChargingStationResponseDto>(updatedStation);
        }
    }
}