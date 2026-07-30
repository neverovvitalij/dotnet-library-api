

using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface ITokenService
{
    string GenerateAccessToken(User  user);
    string GenerateRefreshToken();
}
