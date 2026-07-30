using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly LibraryDbContext _libraryDbContext;
    public UserRepository(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _libraryDbContext.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
    }
    public async Task AddAsync(User user)
    {
        await _libraryDbContext.Users.AddAsync(user);
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await _libraryDbContext.SaveChangesAsync() > 0;
    }
}
