using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public AuthenticationService(
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

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
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

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
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

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.ClearRefreshToken();
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                return _jwtService.ValidateToken(token);
            }
            catch
            {
                return false;
            }
        }
    }
}
