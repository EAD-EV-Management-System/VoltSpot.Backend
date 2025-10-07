using Application.DTOs.Response.ChargingStations;
using Application.UseCases.ChargingStations.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using MongoDB.Bson;

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
            // Validate ObjectId format
            if (!ObjectId.TryParse(request.Id, out _))
            {
                return null;
            }

            var station = await _repository.GetByIdAsync(request.Id);
            return station == null ? null : _mapper.Map<ChargingStationResponseDto>(station);
        }
    }
}