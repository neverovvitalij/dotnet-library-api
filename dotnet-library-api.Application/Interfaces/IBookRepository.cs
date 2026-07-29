using dotnet_library_api.Domain.Models;
using dotnet_library_api.Application.Common;

namespace dotnet_library_api.Application.Interfaces;
public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
    Task<bool> SaveChangesAsync();
    void Delete(Book book);
    Task<PagedResult<Book>> GetPagedAsync(int page, int pageSize);
}
