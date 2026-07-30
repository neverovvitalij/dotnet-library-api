using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> SaveChangesAsync();
}
