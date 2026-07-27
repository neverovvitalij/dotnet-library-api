using dotnet_library_api.Domain.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Infrastructure.Data;

namespace dotnet_library_api.Infrastructure.Repositories;
public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _libraryDbContext;
    public AuthorRepository(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _libraryDbContext.Authors.FindAsync(id);

    }
}
