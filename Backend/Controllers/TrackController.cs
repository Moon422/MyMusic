using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Services;
using MyMusic.ViewModels;

[ApiController]
[Route("api/[controller]")]
public class TrackController : ControllerBase
{
    private readonly ITrackService trackService;

    public TrackController(ITrackService trackService)
    {
        this.trackService = trackService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrackById(int id)
    {
        try
        {
            return Ok(await trackService.GetTrackById(id));
        }
        catch (TrackNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTrack()
    {
        try
        {
            return Ok(await trackService.GetAllTrack());
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPost, Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> CreateTrack([FromBody] CreateTrackDto createTrack)
    {
        try
        {
            var track = await trackService.CreateTrack(createTrack);
            return CreatedAtAction(nameof(GetTrackById), new { id = track.Id }, track);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }
}
