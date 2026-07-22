using dotnet_library_api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_library_api.DTOs;
using dotnet_library_api.Models;
namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly LibraryDbContext _libraryDbContext;
    public AuthorsController(LibraryDbContext libraryDbContext) {  _libraryDbContext = libraryDbContext; }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var authors = await _libraryDbContext.Authors.Select(a => new AuthorDto(a.Id, a.Name, a.Books.Count)).ToListAsync();
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthorById(int id)
    {
        var author = await _libraryDbContext.Authors.Where(a => a.Id == id).Select(a => new AuthorDto(a.Id, a.Name, a.Books.Count)).FirstOrDefaultAsync();
        if(author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }
        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto createAuthorDto)
    {
        var author = new Author
        {
            Name = createAuthorDto.Name,
        };

        _libraryDbContext.Authors.Add(author);
        await _libraryDbContext.SaveChangesAsync();

        var authorDto = new AuthorDto(author.Id, author.Name, 0);
        return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, authorDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(int id, CreateAuthorDto createAuthorDto)
    {
        var author = await _libraryDbContext.Authors.Include(a => a.Books).Where(a => a.Id == id).FirstOrDefaultAsync();
        if (author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }
        else
        {
            author.Name = createAuthorDto.Name;
            await _libraryDbContext.SaveChangesAsync();
            var authorDto = new AuthorDto(author.Id, author.Name, author.Books.Count());
            return Ok(authorDto);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuthor(int id)
    {
        var author = await _libraryDbContext.Authors.FindAsync(id);
        if(author == null)
        {
            return NotFound("Author wurde nicht gefunden");
        }
        else
        {
            _libraryDbContext.Authors.Remove(author);
            await _libraryDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
