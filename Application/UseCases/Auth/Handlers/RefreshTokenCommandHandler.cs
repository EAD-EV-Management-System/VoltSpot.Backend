using Application.DTOs.Response.Auth;
using Application.Interfaces.Services;
using Application.UseCases.Auth.Commands;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using System.Security.Claims;

namespace Application.UseCases.Auth.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IJwtService jwtService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.RefreshToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            // Check account status
            if (user.Status != AccountStatus.Active)
            {
                throw new UnauthorizedAccessException("Account is not active");
            }

            // Generate new tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshExpiry = _jwtService.GetRefreshTokenExpiration();

            // Update user with new refresh token
            user.SetRefreshToken(refreshToken, refreshExpiry);
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
