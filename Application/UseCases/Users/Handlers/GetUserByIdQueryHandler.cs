using Application.DTOs.Response.Users;
using Application.UseCases.Users.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponseDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }
    }
}
