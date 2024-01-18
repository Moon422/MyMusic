using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Services;

[ApiController]
[Route("api/[controller]")]
public class ArtistController : ControllerBase
{
    private readonly IArtistService artistService;

    public ArtistController(IArtistService artistService)
    {
        this.artistService = artistService;
    }

    [HttpGet, Authorize(Roles = "ARTIST")]
    public async Task<IActionResult> Get()
    {
        try
        {
            return Ok(await artistService.Get());
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("requestupgrade"), Authorize(Roles = "LISTENER")]
    public async Task<IActionResult> RequestProfileToArtistUpgrade()
    {
        try
        {
            await artistService.RequestProfileToArtistUpgrade();
            return Ok("Request submitted");
        }
        catch (ProfileCannotUpgradeToArtistException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ProfileUpgradeToArtistRequestExistsException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("getpendingrequests"), Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetPendingProfileToArtistRequests()
    {
        try
        {
            return Ok(await artistService.GetPendingProfileToArtistRequests());
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }
}
