using Application.DTOs.Response.Users;
using Application.UseCases.Users.Commands;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IChargingStationRepository _stationRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IChargingStationRepository stationRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _stationRepository = stationRepository;
            _mapper = mapper;
        }
        public async Task<UserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Check if email is being changed and if it's already taken
            if (user.Email != request.Email && await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Track old station assignments for syncing
            var oldStationIds = user.AssignedStationIds ?? new List<string>();
            var newStationIds = request.AssignedStationIds ?? new List<string>();

            // Update user properties
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Role = request.Role;
            user.Status = request.Status;
            user.AssignedStationIds = newStationIds;

            var updatedUser = await _userRepository.UpdateAsync(user);

            // Sync changes to charging stations
            // Remove operator from stations that are no longer assigned
            var stationsToRemoveFrom = oldStationIds.Except(newStationIds).ToList();
            foreach (var stationId in stationsToRemoveFrom)
            {
                var station = await _stationRepository.GetByIdAsync(stationId);
                if (station != null)
                {
                    station.UnassignOperator(user.Id);
                    await _stationRepository.UpdateAsync(station);
                }
            }

            // Add operator to newly assigned stations
            var stationsToAddTo = newStationIds.Except(oldStationIds).ToList();
            foreach (var stationId in stationsToAddTo)
            {
                var station = await _stationRepository.GetByIdAsync(stationId);
                if (station != null)
                {
                    station.AssignOperator(user.Id);
                    await _stationRepository.UpdateAsync(station);
                }
            }

            return _mapper.Map<UserResponseDto>(updatedUser);
        }
    }
}
