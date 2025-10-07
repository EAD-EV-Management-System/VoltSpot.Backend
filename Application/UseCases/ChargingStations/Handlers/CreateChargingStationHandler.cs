using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class CreateChargingStationHandler : IRequestHandler<CreateChargingStationCommand, ChargingStationResponseDto>
    {
        private readonly IChargingStationRepository _repository;
        private readonly IMapper _mapper;

        public CreateChargingStationHandler(IChargingStationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ChargingStationResponseDto> Handle(CreateChargingStationCommand request, CancellationToken cancellationToken)
        {
            var station = new ChargingStation
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
                OperatingHours = new OperatingHours
                {
                    OpenTime = request.OperatingHours.OpenTime,
                    CloseTime = request.OperatingHours.CloseTime,
                    Is24Hours = request.OperatingHours.Is24Hours,
                    ClosedDays = request.OperatingHours.ClosedDays
                },
                CreatedAt = DateTime.UtcNow
            };

            var createdStation = await _repository.AddAsync(station);
            return _mapper.Map<ChargingStationResponseDto>(createdStation);
        }
    }
}