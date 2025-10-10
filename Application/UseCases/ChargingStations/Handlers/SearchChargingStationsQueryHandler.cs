using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class SearchChargingStationsQueryHandler : IRequestHandler<SearchChargingStationsQuery, List<ChargingStationResponseDto>>
    {
        private readonly IChargingStationRepository _repository;
        private readonly IMapper _mapper;

        public SearchChargingStationsQueryHandler(IChargingStationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ChargingStationResponseDto>> Handle(SearchChargingStationsQuery request, CancellationToken cancellationToken)
        {
            var stations = await _repository.SearchAsync(
                location: request.Location,
                type: request.Type,
                latitude: request.Latitude,
                longitude: request.Longitude,
                radiusKm: request.RadiusKm,
                availableOnly: request.AvailableOnly,
                maxPricePerHour: request.MaxPricePerHour,
                page: request.Page,
                pageSize: request.PageSize);

            return _mapper.Map<List<ChargingStationResponseDto>>(stations);
        }
    }
}