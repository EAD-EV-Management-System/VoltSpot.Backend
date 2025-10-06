using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ChargingStations.Handlers
{
    public class GetChargingStationByIdHandler : IRequestHandler<GetChargingStationByIdQuery, ChargingStationResponseDto?>
    {
        private readonly IChargingStationRepository _repository;
        private readonly IMapper _mapper;

        public GetChargingStationByIdHandler(IChargingStationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ChargingStationResponseDto?> Handle(GetChargingStationByIdQuery request, CancellationToken cancellationToken)
        {
            var station = await _repository.GetByIdAsync(request.Id);
            return station == null ? null : _mapper.Map<ChargingStationResponseDto>(station);
        }
    }
}