using Asp.Versioning;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.DTOs.V1;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_library_api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/books")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IGenreRepository _genreRepository;
    public BooksController(IBookRepository bookRepository, IAuthorRepository authorRepository, IGenreRepository genreRepository) 
    { 
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _genreRepository = genreRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        var books = await _bookRepository.GetAllAsync();
        var booksDto = books.Select(b => new BookDto(b.Id, b.Title, b.PublishedYear, b.Author.Name, b.Genres.Select(g => g.Name).ToList())).ToList();
        return Ok(booksDto);

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBookById(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound("Buch wurde nicht gefunden");
        }
        var bookDto = new BookDto(book.Id, book.Title, book.PublishedYear, book.Author.Name, book.Genres.Select(g => g.Name).ToList());
     
        return Ok(bookDto);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        var author = await _authorRepository.GetByIdAsync(createBookDto.AuthorId);
        if (author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }

        var genreList = await _genreRepository.GetByIdsAsync(createBookDto.GenreIds);

        var book = new Book
        {
            AuthorId = createBookDto.AuthorId,
            Title = createBookDto.Title,
            PublishedYear = createBookDto.PublishedYear,
            Genres = genreList
        };
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();

        var bookDto = new BookDto(book.Id, book.Title, book.PublishedYear, author.Name, book.Genres.Select(g => g.Name).ToList());
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, bookDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, CreateBookDto createBookDto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        var author = await _authorRepository.GetByIdAsync(createBookDto.AuthorId);

        if (book == null || author == null)
        {
            return NotFound("Buch/Author wurde nicht gefunden");
        }

        var genresList = await _genreRepository.GetByIdsAsync(createBookDto.GenreIds);

        book.AuthorId = createBookDto.AuthorId;
        book.Title = createBookDto.Title;
        book.PublishedYear = createBookDto.PublishedYear;
        book.Genres = genresList;
        await _bookRepository.SaveChangesAsync();

        var bookDto = new BookDto(book.Id, book.Title, book.PublishedYear, author.Name, book.Genres.Select(g => g.Name).ToList());
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
