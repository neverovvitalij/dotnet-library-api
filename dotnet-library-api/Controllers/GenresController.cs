using Microsoft.AspNetCore.Mvc;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.DTOs.V1;
using Microsoft.AspNetCore.Authorization;


namespace dotnet_library_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreRepository _genreRepository;
    public GenresController(IGenreRepository genreRepository) {  _genreRepository = genreRepository; }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenre()
    {
        var genres = await _genreRepository.GetAllAsync();
        var genresDto = genres.Select(g => new GenreDto(g.Id, g.Name));
        return Ok(genresDto);

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDto>> GetGenreById(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if(genre == null)
        {
            return NotFound("Genre wurde nicht gefunden");
        }
        var genreDto = new GenreDto(genre.Id, genre.Name);
        return Ok(genreDto);
    }

    [HttpPost]
    [Authorize(Roles ="Admin")]
    public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto createGenreDto)
    {
        var genre = new Genre
        {
            Name = createGenreDto.Name,
        };
        await _genreRepository.AddAsync(genre);
        await _genreRepository.SaveChangesAsync();

        var genreDto = new GenreDto(genre.Id, genre.Name);
        return CreatedAtAction(nameof(GetGenreById), new {id  = genre.Id}, genreDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenreDto>> UpdateGenre(int id, CreateGenreDto createGenreDto)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if(genre == null)
        {
            return NotFound("Genre wurde nicht gefunden");
        }
        else
        {
            genre.Name = createGenreDto.Name;
            await _genreRepository.SaveChangesAsync();
            var genreDto = new GenreDto(genre.Id, genre.Name);
            return Ok(genreDto);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteGenre(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if(genre == null)
        {
            return NotFound("Genre wurde nicht gefunden");
        }
        else
        {
            _genreRepository.Delete(genre);
            await _genreRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
