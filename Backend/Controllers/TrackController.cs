using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Services;
using MyMusic.ViewModels;
using MyMusic.ViewModels.Enums;

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

    [HttpPatch("{trackId}/album/add/{albumId}"), Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> AddTrackToAlbum(int trackId, int albumId)
    {
        try
        {
            ReadTrackDto track;

            if (Enum.TryParse<ProfileTypes>(HttpContext.User.FindFirstValue(ClaimTypes.Role), out var type) && type == ProfileTypes.ADMIN)
            {
                track = await trackService.AddTrackToAlbum(trackId, albumId, true);
            }
            else
            {
                track = await trackService.AddTrackToAlbum(trackId, albumId);
            }

            return Ok(track);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPatch("{trackId}/album/remove"), Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> RemoveTrackFromAlbum(int trackId)
    {
        try
        {
            ReadTrackDto track;

            if (Enum.TryParse<ProfileTypes>(HttpContext.User.FindFirstValue(ClaimTypes.Role), out var type) && type == ProfileTypes.ADMIN)
            {
                track = await trackService.RemoveTrackFromAlbum(trackId, true);
            }
            else
            {
                track = await trackService.RemoveTrackFromAlbum(trackId, false);
            }

            return Ok(track);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPatch("{trackId}/artist/add/{trackId}"), Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> AddArtistToTrack(int trackId, int artistId)
    {
        try
        {
            ReadTrackDto track;

            if (Enum.TryParse<ProfileTypes>(HttpContext.User.FindFirstValue(ClaimTypes.Role), out var type) && type == ProfileTypes.ADMIN)
            {
                track = await trackService.AddTrackToAlbum(trackId, artistId, true);
            }
            else
            {
                track = await trackService.AddTrackToAlbum(trackId, artistId);
            }

            return Ok(track);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPatch("{trackId}/artist/remove"), Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> RemoveArtistfromTrack(int trackId, int artistId)
    {
        try
        {
            ReadTrackDto track;

            if (Enum.TryParse<ProfileTypes>(HttpContext.User.FindFirstValue(ClaimTypes.Role), out var type) && type == ProfileTypes.ADMIN)
            {
                track = await trackService.RemoveArtistfromTrack(trackId, artistId, true);
            }
            else
            {
                track = await trackService.RemoveArtistfromTrack(trackId, artistId);
            }

            return Ok(track);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPatch("{trackId}/genre/add/{genreId}"), Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> AddGenreToTrack(int trackId, int genreId)
    {
        try
        {
            ReadTrackDto track;

            if (Enum.TryParse<ProfileTypes>(HttpContext.User.FindFirstValue(ClaimTypes.Role), out var type) && type == ProfileTypes.ADMIN)
            {
                track = await trackService.AddGenreToTrack(trackId, genreId, true);
            }
            else
            {
                track = await trackService.AddGenreToTrack(trackId, genreId, false);
            }

            return Ok(track);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPatch("{trackId}/genre/add/{genreId}"), Authorize(Roles = "ADMIN,ARTIST")]
    public async Task<IActionResult> RemoveGenreToTrack(int trackId, int genreId)
    {
        try
        {
            ReadTrackDto track;

            if (Enum.TryParse<ProfileTypes>(HttpContext.User.FindFirstValue(ClaimTypes.Role), out var type) && type == ProfileTypes.ADMIN)
            {
                track = await trackService.RemoveGenreFromTrack(trackId, genreId, true);
            }
            else
            {
                track = await trackService.RemoveGenreFromTrack(trackId, genreId, false);
            }

            return Ok(track);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }
}
