using Application.UseCases.Users.Commands;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                return false;
            }

            // Soft delete
            await _userRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}
