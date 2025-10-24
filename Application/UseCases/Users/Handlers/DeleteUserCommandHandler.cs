using Application.UseCases.Users.Commands;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IChargingStationRepository _stationRepository;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IChargingStationRepository stationRepository)
        {
            _userRepository = userRepository;
            _stationRepository = stationRepository;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                return false;
            }

            // Remove operator from all assigned charging stations
            if (user.AssignedStationIds != null && user.AssignedStationIds.Any())
            {
                foreach (var stationId in user.AssignedStationIds)
                {
                    var station = await _stationRepository.GetByIdAsync(stationId);
                    if (station != null)
                    {
                        station.UnassignOperator(user.Id);
                        await _stationRepository.UpdateAsync(station);
                    }
                }
            }

            // Soft delete the user
            await _userRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}
