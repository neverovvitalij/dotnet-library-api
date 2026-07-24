using dotnet_library_api.Data;
using dotnet_library_api.DTOs;
using dotnet_library_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _libraryDbContext;
    public BooksController(LibraryDbContext libraryDbContext) { _libraryDbContext = libraryDbContext; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetBooks()
    {
        var books = await _libraryDbContext.Books.Select(b =>
        new BookDto(b.Id, b.Title, b.PublishedYear, b.Author.Name, b.Genres.Select(g => g.Name).ToList())
        ).ToListAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBookById(int id)
    {
        var book = await _libraryDbContext.Books.Where(b => b.Id == id).Select(b =>
        new BookDto(b.Id, b.Title, b.PublishedYear, b.Author.Name, b.Genres.Select(g => g.Name).ToList())
        ).FirstOrDefaultAsync();
        if (book == null)
        {
            return NotFound("Buch wurde nicht gefunden");
        }
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        var author = await _libraryDbContext.Authors.FindAsync(createBookDto.AuthorId);
        if (author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }

        var genreList = await _libraryDbContext.Genres.Where(g => createBookDto.GenreIds.Contains(g.Id)).ToListAsync();

        var book = new Book
        {
            AuthorId = createBookDto.AuthorId,
            Title = createBookDto.Title,
            PublishedYear = createBookDto.PublishedYear,
            Genres = genreList
        };
        _libraryDbContext.Books.Add(book);
        await _libraryDbContext.SaveChangesAsync();

        var bookDto = new BookDto(book.Id, book.Title, book.PublishedYear, author.Name, book.Genres.Select(g => g.Name).ToList());
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, bookDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, CreateBookDto createBookDto)
    {
        var book = await _libraryDbContext.Books.Include(b => b.Genres).Where(b => b.Id == id).SingleOrDefaultAsync();
        var author = await _libraryDbContext.Authors.Where(a => a.Id == createBookDto.AuthorId).SingleOrDefaultAsync();

        if (book == null || author == null)
        {
            return NotFound("Buch/Author wurde nicht gefunden");
        }

        var genresList = await _libraryDbContext.Genres.Where(g => createBookDto.GenreIds.Contains(g.Id)).ToListAsync();

        book.AuthorId = createBookDto.AuthorId;
        book.Title = createBookDto.Title;
        book.PublishedYear = createBookDto.PublishedYear;
        book.Genres = genresList;
        await _libraryDbContext.SaveChangesAsync();

        var bookDto = new BookDto(book.Id, book.Title, book.PublishedYear, author.Name, book.Genres.Select(g => g.Name).ToList());
        return Ok(bookDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var book = await _libraryDbContext.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound("Buch wurde nicht gefunden");
        }
        _libraryDbContext.Books.Remove(book);
        await _libraryDbContext.SaveChangesAsync();
        return NoContent();
    }
}
