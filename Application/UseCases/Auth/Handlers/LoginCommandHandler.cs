using Application.DTOs.Response.Auth;
using Application.Interfaces.Services;
using Application.UseCases.Auth.Commands;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IJwtService jwtService,
            IPasswordService passwordService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find user by username
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Check account status
            if (user.Status != AccountStatus.Active)
            {
                throw new UnauthorizedAccessException("Account is not active");
            }

            // Verify password
            if (!_passwordService.VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshExpiry = _jwtService.GetRefreshTokenExpiration();

            // Update user with refresh token and last login
            user.SetRefreshToken(refreshToken, refreshExpiry);
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = _jwtService.GetTokenExpiration(),
                RefreshTokenExpiry = refreshExpiry,
                User = _mapper.Map<UserInfoResponseDto>(user)
            };
        }
    }
}
