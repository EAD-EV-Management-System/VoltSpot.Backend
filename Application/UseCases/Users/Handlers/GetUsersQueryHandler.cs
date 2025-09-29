using Application.DTOs.Response.Users;
using Application.UseCases.Users.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Handlers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.User> users;

            if (request.Role.HasValue)
            {
                users = await _userRepository.GetByRoleAsync(request.Role.Value);
            }
            else
            {
                users = await _userRepository.GetAllAsync();
            }

            // Apply additional filters
            if (request.Status.HasValue)
            {
                users = users.Where(u => u.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                users = users.Where(u =>
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm));
            }

            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
    }
}
