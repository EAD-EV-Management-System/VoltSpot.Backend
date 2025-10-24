using Application.DTOs.Response.EVOwners;
using Application.UseCases.EVOwners.Commands;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class ActivateEVOwnerCommandHandler : IRequestHandler<ActivateEVOwnerCommand, EVOwnerResponseDto>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IMapper _mapper;

        public ActivateEVOwnerCommandHandler(
            IEVOwnerRepository evOwnerRepository,
            IMapper mapper)
        {
            _evOwnerRepository = evOwnerRepository;
            _mapper = mapper;
        }

        public async Task<EVOwnerResponseDto> Handle(
            ActivateEVOwnerCommand request,
            CancellationToken cancellationToken)
        {
            var evOwner = await _evOwnerRepository.GetByNICAsync(request.NIC);

            if (evOwner == null)
            {
                throw new KeyNotFoundException($"EV Owner with NIC {request.NIC} not found");
            }

            if (evOwner.Status == AccountStatus.Active)
            {
                throw new InvalidOperationException("EV Owner account is already active");
            }

            // Activate the account
            evOwner.Status = AccountStatus.Active;
            evOwner.UpdatedAt = DateTime.UtcNow;

            var updatedEVOwner = await _evOwnerRepository.UpdateAsync(evOwner);
            return _mapper.Map<EVOwnerResponseDto>(updatedEVOwner);
        }
    }
}
