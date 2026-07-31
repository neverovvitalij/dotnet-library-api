using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Infrastructure.Repositories;
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly LibraryDbContext _libraryDbContext;
    public RefreshTokenRepository(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }
    public async Task AddNewTokenAsync(RefreshToken token)
    {
        await _libraryDbContext.RefreshTokens.AddAsync(token);
    }
    public async Task<RefreshToken?> GetByTokenAsync(string refreshToken)
    {
        return await _libraryDbContext.RefreshTokens.Include(t => t.User).Where(t => t.Token == refreshToken).FirstOrDefaultAsync();
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await _libraryDbContext.SaveChangesAsync() > 0;
    }
}
