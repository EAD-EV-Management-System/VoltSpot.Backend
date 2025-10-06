using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class GetAllChargingStationsHandler : IRequestHandler<GetAllChargingStationsQuery, List<ChargingStationResponseDto>>
    {
        private readonly IChargingStationRepository _repository;
        private readonly IMapper _mapper;

        public GetAllChargingStationsHandler(IChargingStationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ChargingStationResponseDto>> Handle(GetAllChargingStationsQuery request, CancellationToken cancellationToken)
        {
            var stations = request.OnlyActive 
                ? await _repository.GetActiveStationsAsync()
                : (await _repository.GetAllAsync()).ToList();

            return _mapper.Map<List<ChargingStationResponseDto>>(stations);
        }
    }
}