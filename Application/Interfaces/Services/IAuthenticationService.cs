using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;

namespace Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> LogoutAsync(string userId);
        Task<bool> ValidateTokenAsync(string token);
    }
}
