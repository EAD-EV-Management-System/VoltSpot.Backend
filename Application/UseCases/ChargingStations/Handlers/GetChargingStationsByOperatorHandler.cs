using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class GetChargingStationsByOperatorHandler : IRequestHandler<GetChargingStationsByOperatorQuery, List<ChargingStationResponseDto>>
    {
        private readonly IChargingStationRepository _repository;
        private readonly IMapper _mapper;

        public GetChargingStationsByOperatorHandler(IChargingStationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ChargingStationResponseDto>> Handle(GetChargingStationsByOperatorQuery request, CancellationToken cancellationToken)
        {
            var stations = await _repository.GetStationsByOperatorAsync(request.OperatorId);
            return _mapper.Map<List<ChargingStationResponseDto>>(stations);
        }
    }
}
