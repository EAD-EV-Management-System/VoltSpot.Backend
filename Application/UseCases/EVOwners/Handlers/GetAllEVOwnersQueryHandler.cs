using Application.DTOs.Response.EVOwners;
using Application.UseCases.EVOwners.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class GetAllEVOwnersQueryHandler : IRequestHandler<GetAllEVOwnersQuery, List<EVOwnerResponseDto>>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IMapper _mapper;

        public GetAllEVOwnersQueryHandler(
            IEVOwnerRepository evOwnerRepository,
            IMapper mapper)
        {
            _evOwnerRepository = evOwnerRepository;
            _mapper = mapper;
        }

        public async Task<List<EVOwnerResponseDto>> Handle(
            GetAllEVOwnersQuery request,
            CancellationToken cancellationToken)
        {
            var evOwners = await _evOwnerRepository.GetAllAsync(
                request.Page,
                request.PageSize,
                request.Status,
                request.SearchTerm);

            return _mapper.Map<List<EVOwnerResponseDto>>(evOwners);
        }
    }
}