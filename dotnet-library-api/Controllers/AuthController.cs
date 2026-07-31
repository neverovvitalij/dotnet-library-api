using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using dotnet_library_api.Application.Common;
using Microsoft.Extensions.Options;

namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;
    public AuthController(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IOptions<JwtSettings>  jwtOptions)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        var isAlreadyRegistered = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if (isAlreadyRegistered != null)
        {
            return Conflict("UserName ist bereits vergeben");
        }
        var user = new User
        {
            Username = registerDto.Username,
            Role = "User"
        };
        var hasher = new PasswordHasher<User>();
        var hashedPassword = hasher.HashPassword(user, registerDto.Password);
        user.PasswordHash = hashedPassword;
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenModel = new RefreshToken
        {
            Token = refreshToken,
            User = user,
            ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false
        };
        await _refreshTokenRepository.AddNewTokenAsync(refreshTokenModel);
        await _refreshTokenRepository.SaveChangesAsync();

        var authResponseDto = new AuthResponseDto(refreshToken, accessToken);
        return Ok(authResponseDto);
    }
}
