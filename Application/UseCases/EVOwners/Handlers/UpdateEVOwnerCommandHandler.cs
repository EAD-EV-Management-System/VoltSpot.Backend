using Application.DTOs.Response.EVOwners;
using Application.UseCases.EVOwners.Commands;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class UpdateEVOwnerCommandHandler : IRequestHandler<UpdateEVOwnerCommand, EVOwnerResponseDto>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IMapper _mapper;

        public UpdateEVOwnerCommandHandler(
            IEVOwnerRepository evOwnerRepository,
            IMapper mapper)
        {
            _evOwnerRepository = evOwnerRepository;
            _mapper = mapper;
        }

        public async Task<EVOwnerResponseDto> Handle(
            UpdateEVOwnerCommand request,
            CancellationToken cancellationToken)
        {
            var evOwner = await _evOwnerRepository.GetByNICAsync(request.NIC);
            
            if (evOwner == null)
            {
                throw new KeyNotFoundException($"EV Owner with NIC {request.NIC} not found");
            }

            // Update fields
            evOwner.FirstName = request.FirstName;
            evOwner.LastName = request.LastName;
            evOwner.Email = request.Email;
            evOwner.PhoneNumber = request.PhoneNumber;
            evOwner.UpdatedAt = DateTime.UtcNow;

            var updatedEVOwner = await _evOwnerRepository.UpdateAsync(evOwner);
            return _mapper.Map<EVOwnerResponseDto>(updatedEVOwner);
        }
    }
}