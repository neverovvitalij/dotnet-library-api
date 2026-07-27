using dotnet_library_api.Domain.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Infrastructure.Repositories;
public class GenreRepository :IGenreRepository
{
    private readonly LibraryDbContext _libraryDbContext;
    public GenreRepository(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<List<Genre>> GetByIdsAsync(List<int> ids)
    {
        return await _libraryDbContext.Genres.Where(g => ids.Contains(g.Id)).ToListAsync();
    }
}
