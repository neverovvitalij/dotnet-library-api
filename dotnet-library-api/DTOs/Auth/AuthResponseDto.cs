namespace dotnet_library_api.DTOs.Auth;

public record AuthResponseDto
(
    string RefreshToken,
    string AccessToken
);

