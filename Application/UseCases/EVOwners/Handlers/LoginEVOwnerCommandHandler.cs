using Application.DTOs.Response.EVOwners;
using Application.Interfaces.Services;
using Application.UseCases.EVOwners.Commands;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class LoginEVOwnerCommandHandler : IRequestHandler<LoginEVOwnerCommand, EVOwnerLoginResponseDto>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public LoginEVOwnerCommandHandler(
            IEVOwnerRepository evOwnerRepository,
            IJwtService jwtService,
            IPasswordService passwordService,
            IMapper mapper)
        {
            _evOwnerRepository = evOwnerRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<EVOwnerLoginResponseDto> Handle(LoginEVOwnerCommand request, CancellationToken cancellationToken)
        {
            // Find EV owner by NIC
            var evOwner = await _evOwnerRepository.GetByNICAsync(request.NIC);
            if (evOwner == null)
            {
                throw new UnauthorizedAccessException("Invalid NIC or password");
            }

            // Check account status
            if (evOwner.Status != AccountStatus.Active)
            {
                throw new UnauthorizedAccessException("Account is not active");
            }

            // Verify password
            if (!_passwordService.VerifyPassword(request.Password, evOwner.Password))
            {
                throw new UnauthorizedAccessException("Invalid NIC or password");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(evOwner);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshExpiry = _jwtService.GetRefreshTokenExpiration();

            // Update EV owner with refresh token and last login
            evOwner.SetRefreshToken(refreshToken, refreshExpiry);
            evOwner.UpdateLastLogin();
            await _evOwnerRepository.UpdateAsync(evOwner);

            return new EVOwnerLoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = _jwtService.GetTokenExpiration(),
                RefreshTokenExpiry = refreshExpiry,
                EVOwner = _mapper.Map<EVOwnerResponseDto>(evOwner)
            };
        }
    }
}
