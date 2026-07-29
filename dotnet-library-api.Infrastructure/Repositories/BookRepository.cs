using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Infrastructure.Data;
using dotnet_library_api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using dotnet_library_api.Application.Common;


namespace dotnet_library_api.Infrastructure.Repositories;
public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _libraryDbContext;
    public BookRepository(LibraryDbContext libraryDbContext) {  _libraryDbContext = libraryDbContext; }

    public async Task<List<Book>> GetAllAsync()
    {
        var books = await _libraryDbContext.Books.Include(b => b.Genres).Include(b => b.Author).ToListAsync();
        return books;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        var book = await _libraryDbContext.Books.Include(b => b.Genres).Include(b => b.Author).Where(b => b.Id == id).FirstOrDefaultAsync();
        return book;
    }

    public async Task AddAsync(Book book)
    {
        await _libraryDbContext.Books.AddAsync(book);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _libraryDbContext.SaveChangesAsync() > 0;
    }

    public void Delete(Book book)
    {
        _libraryDbContext.Books.Remove(book);
    }
    public async Task<PagedResult<Book>> GetPagedAsync(int page, int pageSize)
    {
        var totalCount = await _libraryDbContext.Books.CountAsync();
        var books = await _libraryDbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Genres)
            .OrderBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedResult<Book>(books, totalCount);
    }
}