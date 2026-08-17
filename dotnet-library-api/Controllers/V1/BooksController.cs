using Asp.Versioning;
using dotnet_library_api.Application.Books.Commands;
using dotnet_library_api.Application.Books.Queries;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.DTOs.V1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IMediator _mediator;
    public BooksController(IBookRepository bookRepository, IAuthorRepository authorRepository, IGenreRepository genreRepository, IMediator mediator) 
    { 
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _genreRepository = genreRepository;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(int page = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetBooksQuery(page, pageSize));
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        var booksDto = result.Items.Select(b => new BookDto(b.Id, b.Title, b.PublishedYear, b.AuthorName, b.Genres)).ToList();
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        var result = await _mediator.Send(new CreateBookCommand(createBookDto.Title, createBookDto.PublishedYear, createBookDto.AuthorId, createBookDto.GenreIds));
        if (result == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }

        var bookDto = new BookDto(result.Id, result.Title, result.PublishedYear, result.AuthorName, result.Genres);
        return CreatedAtAction(nameof(GetBookById), new { id = result.Id }, bookDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
