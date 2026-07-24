using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
    Task<bool> SaveChangesAsync();
    void Delete(Book book);
}
