using Microsoft.AspNetCore.Mvc;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.DTOs.V1;
namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorRepository _authorRepository;
    public AuthorsController(IAuthorRepository authorRepository) {  _authorRepository = authorRepository; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var authors = await _authorRepository.GetAllAsync();
        var authorsDto = authors.Select(a => new AuthorDto(a.Id, a.Name, a.Books.Count)).ToList();
        return Ok(authorsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthorById(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if(author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }
        var authorDto = new AuthorDto(author.Id, author.Name, author.Books.Count);
        return Ok(authorDto);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto createAuthorDto)
    {
        var author = new Author
        {
            Name = createAuthorDto.Name,
        };

        await _authorRepository.AddAsync(author);
        await _authorRepository.SaveChangesAsync();

        var authorDto = new AuthorDto(author.Id, author.Name, 0);
        return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, authorDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(int id, CreateAuthorDto createAuthorDto)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }
        else
        {
            author.Name = createAuthorDto.Name;
            await _authorRepository.SaveChangesAsync();
            var authorDto = new AuthorDto(author.Id, author.Name, author.Books.Count());
            return Ok(authorDto);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuthor(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if(author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }
        else
        {
            _authorRepository.Delete(author);
            await _authorRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
