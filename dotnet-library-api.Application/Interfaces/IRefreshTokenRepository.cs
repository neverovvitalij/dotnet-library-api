using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface IRefreshTokenRepository
{
    Task AddNewTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string refreshToken);
    Task<bool> SaveChangesAsync();
}
