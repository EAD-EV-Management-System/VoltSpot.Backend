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
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
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

            // Update user properties
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Role = request.Role;
            user.Status = request.Status;
            user.AssignedStationIds = request.AssignedStationIds;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserResponseDto>(updatedUser);
        }
    }
}
