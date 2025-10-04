using Application.UseCases.Auth.Commands;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Auth.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public LogoutCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return false;

            user.ClearRefreshToken();
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
