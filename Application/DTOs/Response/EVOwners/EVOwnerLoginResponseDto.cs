namespace Application.DTOs.Response.EVOwners
{
    public class EVOwnerLoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public EVOwnerResponseDto EVOwner { get; set; } = null!;
    }
}
