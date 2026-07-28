using dotnet_library_api.Domain.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Infrastructure.Repositories;
public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _libraryDbContext;
    public AuthorRepository(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<List<Author>> GetAllAsync()
    {
        return await _libraryDbContext.Authors.Include(a => a.Books).ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _libraryDbContext.Authors.Include(a => a.Books).Where(a => a.Id ==id).FirstOrDefaultAsync();

    }

    public async Task AddAsync(Author author)
    {
        await _libraryDbContext.Authors.AddAsync(author);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _libraryDbContext.SaveChangesAsync() > 0;
    }

    public void Delete(Author author)
    {
        _libraryDbContext.Authors.Remove(author);
    }
}
