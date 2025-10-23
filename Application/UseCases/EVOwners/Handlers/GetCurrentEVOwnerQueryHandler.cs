using Application.DTOs.Response.EVOwners;
using Application.UseCases.EVOwners.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class GetCurrentEVOwnerQueryHandler : IRequestHandler<GetCurrentEVOwnerQuery, EVOwnerResponseDto>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IMapper _mapper;

        public GetCurrentEVOwnerQueryHandler(
            IEVOwnerRepository evOwnerRepository,
            IMapper mapper)
        {
            _evOwnerRepository = evOwnerRepository;
            _mapper = mapper;
        }

        public async Task<EVOwnerResponseDto> Handle(
            GetCurrentEVOwnerQuery request,
            CancellationToken cancellationToken)
        {
            var evOwner = await _evOwnerRepository.GetByNICAsync(request.EvOwnerNic);
            
            if (evOwner == null)
            {
                throw new KeyNotFoundException($"EV Owner with NIC {request.EvOwnerNic} not found");
            }

            return _mapper.Map<EVOwnerResponseDto>(evOwner);
        }
    }
}