using Application.DTOs.Response.Users;
using Application.Interfaces.Services;
using Application.UseCases.Users.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IChargingStationRepository _stationRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IChargingStationRepository stationRepository,
            IPasswordService passwordService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _stationRepository = stationRepository;
            _passwordService = passwordService;
            _mapper = mapper;
        }
        public async Task<UserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if username already exists
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Check if email already exists
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = _passwordService.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                AssignedStationIds = request.AssignedStationIds ?? new List<string>()
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Sync operator ID to assigned charging stations
            if (request.AssignedStationIds != null && request.AssignedStationIds.Any())
            {
                foreach (var stationId in request.AssignedStationIds)
                {
                    var station = await _stationRepository.GetByIdAsync(stationId);
                    if (station != null)
                    {
                        station.AssignOperator(createdUser.Id);
                        await _stationRepository.UpdateAsync(station);
                    }
                }
            }

            return _mapper.Map<UserResponseDto>(createdUser);
        }
    }
}
