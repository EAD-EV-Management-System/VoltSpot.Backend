using Application.DTOs.Response.Auth;
using Application.UseCases.Auth.Queries;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Auth.Handlers
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserInfoResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserInfoResponseDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            return _mapper.Map<UserInfoResponseDto>(user);
        }
    }
}
