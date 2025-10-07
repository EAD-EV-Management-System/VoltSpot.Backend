using Domain.Entities;
using System.Security.Claims;

namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateAccessToken(EVOwner evOwner);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
        DateTime GetTokenExpiration();
        DateTime GetRefreshTokenExpiration();
    }
}
