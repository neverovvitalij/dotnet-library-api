using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Infrastructure.Data;
using dotnet_library_api.Domain.Models;
using Microsoft.EntityFrameworkCore;


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
}