using Application.DTOs.Response.EVOwners;
using Application.UseCases.EVOwners.Commands;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class DeactivateEVOwnerCommandHandler : IRequestHandler<DeactivateEVOwnerCommand, EVOwnerResponseDto>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IMapper _mapper;

        public DeactivateEVOwnerCommandHandler(
            IEVOwnerRepository evOwnerRepository,
            IMapper mapper)
        {
            _evOwnerRepository = evOwnerRepository;
            _mapper = mapper;
        }

        public async Task<EVOwnerResponseDto> Handle(
            DeactivateEVOwnerCommand request,
            CancellationToken cancellationToken)
        {
            var evOwner = await _evOwnerRepository.GetByNICAsync(request.NIC);

            if (evOwner == null)
            {
                throw new KeyNotFoundException($"EV Owner with NIC {request.NIC} not found");
            }

            if (evOwner.Status == AccountStatus.Inactive)
            {
                throw new InvalidOperationException("EV Owner account is already inactive");
            }

            // Deactivate the account
            evOwner.Status = AccountStatus.Inactive;
            evOwner.UpdatedAt = DateTime.UtcNow;

            var updatedEVOwner = await _evOwnerRepository.UpdateAsync(evOwner);
            return _mapper.Map<EVOwnerResponseDto>(updatedEVOwner);
        }
    }
}
