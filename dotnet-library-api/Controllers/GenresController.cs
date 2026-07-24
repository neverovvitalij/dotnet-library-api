using dotnet_library_api.Infrastructure.Data;
using dotnet_library_api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_library_api.Domain.Models;


namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly LibraryDbContext _libraryDbContext;
    public GenresController(LibraryDbContext libraryDbContext) {  _libraryDbContext = libraryDbContext; }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenre()
    {
        var genresDto = await _libraryDbContext.Genres.Select(a => new GenreDto(a.Id, a.Name)).ToListAsync();
        return Ok(genresDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDto>> GetGenreById(int id)
    {
        var genreDto = await _libraryDbContext.Genres.Where(a => a.Id == id).Select(a => new GenreDto(a.Id, a.Name)).FirstOrDefaultAsync();
        if(genreDto == null)
        {
            return NotFound("Genre wurde nicht gefunden");
        }
        return Ok(genreDto);
    }

    [HttpPost]
    public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto createGenreDto)
    {
        var genre = new Genre
        {
            Name = createGenreDto.Name,
        };
        _libraryDbContext.Genres.Add(genre);
        await _libraryDbContext.SaveChangesAsync();

        var genreDto = new GenreDto(genre.Id, genre.Name);
        return CreatedAtAction(nameof(GetGenreById), new {id  = genre.Id}, genreDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GenreDto>> UpdateGenre(int id, CreateGenreDto createGenreDto)
    {
        var genre = await _libraryDbContext.Genres.Where(g => g.Id == id).FirstOrDefaultAsync();
        if(genre == null)
        {
            return NotFound("Genre wurde nicht gefunden");
        }
        else
        {
            genre.Name = createGenreDto.Name;
            await _libraryDbContext.SaveChangesAsync();
            var genreDto = new GenreDto(genre.Id, genre.Name);
            return Ok(genreDto);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGenre(int id)
    {
        var genre = await _libraryDbContext.Genres.FindAsync(id);
        if(genre == null)
        {
            return NotFound("Genre wurde nicht gefunden");
        }
        else
        {
            _libraryDbContext.Genres.Remove(genre);
            await _libraryDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
