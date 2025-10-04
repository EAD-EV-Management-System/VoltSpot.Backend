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
            // Push all filtering to database level
            var users = await _userRepository.GetUsersAsync(
                role: request.Role,
                status: request.Status,
                searchTerm: request.SearchTerm
            );

            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
    }
}
