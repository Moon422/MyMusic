using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Services;
using MyMusic.ViewModels;

[ApiController]
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService genreService;

    public GenreController(IGenreService genreService)
    {
        this.genreService = genreService;
    }

    [HttpGet, Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> GetAllGenre()
    {
        try
        {
            return Ok(await genreService.GetAllGenre());
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("{id}"), Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> GetGenreById(int id)
    {
        try
        {
            var genre = await genreService.GetGenreById(id);
            return Ok(genre);
        }
        catch (GenreNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPost, Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreDto createGenre)
    {
        try
        {
            var genre = await genreService.CreateGenre(createGenre);
            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genre);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }
}
