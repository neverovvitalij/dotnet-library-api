

using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync();
    Task<Author?> GetByIdAsync(int id);
    Task AddAsync(Author author);
    Task<bool> SaveChangesAsync();
    void Delete(Author author);
}
