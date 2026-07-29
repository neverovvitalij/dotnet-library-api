using Asp.Versioning;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.DTOs.V2;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_library_api.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/books")]
public class BooksControllerV2 : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IGenreRepository _genreRepository;
    public BooksControllerV2(IBookRepository bookRepository, IAuthorRepository authorRepository, IGenreRepository genreRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _genreRepository = genreRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDtoV2>>> GetBooks(int page = 1, int pageSize = 10)
    {
        var books = await _bookRepository.GetPagedAsync(page, pageSize);
        Response.Headers["X-Total-Count"] = books.TotalCount.ToString();
        var booksDto = books.Items.Select(b => new BookDtoV2(b.Id, b.Title, b.PublishedYear, b.Author.Name, b.Genres.Select(g => g.Name).ToList(), b.Publisher)).ToList();
        return Ok(booksDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDtoV2>> GetBookById(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound("Buch wurde nicht gefunden");
        }
        var bookDto = new BookDtoV2(book.Id, book.Title, book.PublishedYear, book.Author.Name, book.Genres.Select(g => g.Name).ToList(), book.Publisher);

        return Ok(bookDto);
    }

    [HttpPost]
    public async Task<ActionResult<BookDtoV2>> CreateBook(CreateBookDtoV2 createBookDtoV2)
    {
        var author = await _authorRepository.GetByIdAsync(createBookDtoV2.AuthorId);
        if (author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }

        var genreList = await _genreRepository.GetByIdsAsync(createBookDtoV2.GenreIds);

        var book = new Book
        {
            AuthorId = createBookDtoV2.AuthorId,
            Title = createBookDtoV2.Title,
            PublishedYear = createBookDtoV2.PublishedYear,
            Genres = genreList,
            Publisher = createBookDtoV2.Publisher
        };
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();

        var bookDto = new BookDtoV2(book.Id, book.Title, book.PublishedYear, author.Name, book.Genres.Select(g => g.Name).ToList(), book.Publisher);
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, bookDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDtoV2>> UpdateBook(int id, CreateBookDtoV2 createBookDtoV2)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        var author = await _authorRepository.GetByIdAsync(createBookDtoV2.AuthorId);

        if (book == null || author == null)
        {
            return NotFound("Buch/Author wurde nicht gefunden");
        }

        var genresList = await _genreRepository.GetByIdsAsync(createBookDtoV2.GenreIds);

        book.AuthorId = createBookDtoV2.AuthorId;
        book.Title = createBookDtoV2.Title;
        book.PublishedYear = createBookDtoV2.PublishedYear;
        book.Genres = genresList;
        book.Publisher = createBookDtoV2.Publisher;

        await _bookRepository.SaveChangesAsync();

        var bookDto = new BookDtoV2(book.Id, book.Title, book.PublishedYear, author.Name, book.Genres.Select(g => g.Name).ToList(), book.Publisher);
        return Ok(bookDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound("Buch wurde nicht gefunden");
        }
        _bookRepository.Delete(book);
        await _bookRepository.SaveChangesAsync();
        return NoContent();
    }
}
