using Application.DTOs.Response.EVOwners;
using Application.Interfaces.Services;
using Application.UseCases.EVOwners.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class RegisterEVOwnerCommandHandler : IRequestHandler<RegisterEVOwnerCommand, EVOwnerResponseDto>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public RegisterEVOwnerCommandHandler(
            IEVOwnerRepository evOwnerRepository,
            IPasswordService passwordService,
            IMapper mapper)
        {
            _evOwnerRepository = evOwnerRepository;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<EVOwnerResponseDto> Handle(RegisterEVOwnerCommand request, CancellationToken cancellationToken)
        {
            // Check if NIC already exists
            if (await _evOwnerRepository.ExistsByNICAsync(request.NIC))
            {
                throw new InvalidOperationException("An account with this NIC already exists");
            }

            // Check if email already exists
            if (await _evOwnerRepository.ExistsByEmailAsync(request.Email))
            {
                throw new InvalidOperationException("An account with this email already exists");
            }

            // Create new EV owner
            var evOwner = new EVOwner
            {
                NIC = request.NIC,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = _passwordService.HashPassword(request.Password)
            };

            var createdEVOwner = await _evOwnerRepository.AddAsync(evOwner);
            return _mapper.Map<EVOwnerResponseDto>(createdEVOwner);
        }
    }
}
