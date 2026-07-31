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

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        var isAlreadyRegistered = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if(isAlreadyRegistered != null)
        {
            return Conflict("Username ist bereits registriert");
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
            ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            User = user,
            IsRevoked = false
        };
        await _refreshTokenRepository.AddNewTokenAsync(refreshTokenModel);
        await _refreshTokenRepository.SaveChangesAsync();

        var authResponseDto = new AuthResponseDto(refreshToken, accessToken);
        return Ok(authResponseDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null)
        {
            return Unauthorized("Ungültiger Benutzername oder Passwort");
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (result != PasswordVerificationResult.Success)
        {
            return Unauthorized("Ungültiger Benutzername oder Passwort");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenModel = new RefreshToken
        {
            Token = refreshToken,
            ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            User = user,
            IsRevoked = false
        };
        await _refreshTokenRepository.AddNewTokenAsync(refreshTokenModel);
        await _refreshTokenRepository.SaveChangesAsync();

        var authResponseDto = new AuthResponseDto(refreshToken, accessToken);
        return Ok(authResponseDto);
    }
}
