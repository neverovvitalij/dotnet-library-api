
using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface IGenreRepository
{
    Task<List<Genre>> GetAllAsync();
    Task<List<Genre>> GetByIdsAsync(List<int> ids);
    Task<Genre?> GetByIdAsync(int id);
    Task AddAsync(Genre genre);
    Task<bool> SaveChangesAsync();
    void Delete(Genre genre);
}
