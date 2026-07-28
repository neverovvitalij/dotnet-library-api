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

    public async Task<List<Genre>> GetAllAsync()
    {
        return await _libraryDbContext.Genres.ToListAsync();
    }

    public async Task<List<Genre>> GetByIdsAsync(List<int> ids)
    {
        return await _libraryDbContext.Genres.Where(g => ids.Contains(g.Id)).ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _libraryDbContext.Genres.FindAsync(id);
    }

    public async Task AddAsync(Genre genre)
    {
        await _libraryDbContext.Genres.AddAsync(genre);
    } 

    public async Task<bool> SaveChangesAsync()
    {
        return await _libraryDbContext.SaveChangesAsync() > 0;
    }

    public void Delete(Genre genre)
    {
        _libraryDbContext.Genres.Remove(genre);
    }
}
