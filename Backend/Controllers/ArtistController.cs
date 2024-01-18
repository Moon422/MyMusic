using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Services;
using MyMusic.ViewModels.Enums;

[ApiController]
[Route("api/[controller]")]
public class ArtistController : ControllerBase
{
    private readonly IArtistService artistService;

    public ArtistController(IArtistService artistService)
    {
        this.artistService = artistService;
    }

    [HttpGet, Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetArtists()
    {
        try
        {
            return Ok(await artistService.GetArtists());
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("{id}"), Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetArtistById(int id)
    {
        try
        {
            return Ok(await artistService.GetArtistById(id));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("upgrade"), Authorize(Roles = "LISTENER")]
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

    [HttpGet("upgrade/pending"), Authorize(Roles = "ADMIN")]
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

    [HttpPost("upgrade/pending/{id}"), Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> RespondPendingProfileToArtistRequest(int id, [FromBody] ArtistUpgradeRequestStatus status)
    {
        try
        {
            if (status == ArtistUpgradeRequestStatus.APPROVED)
            {
                var artist = await artistService.ApprovePendingProfileToArtistRequest(id);

                return CreatedAtAction(nameof(GetArtistById), new { id = artist.Id }, artist);
            }
            else if (status == ArtistUpgradeRequestStatus.DISAPPROVED)
            {
                await artistService.DisapprovePendingProfileToArtistRequest(id);
                return NoContent();
            }
            else
            {
                return BadRequest("Invalid status");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }
}
