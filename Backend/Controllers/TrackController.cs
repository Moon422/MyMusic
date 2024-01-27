using System;
using System.Collections.Generic;
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
    public async Task<IActionResult> GetAllTrack(int page = 1, int pageSize = 10)
    {
        try
        {
            return Ok(await trackService.GetAllTrack(page, pageSize));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("filter/artist/{artistId}")]
    public async Task<IActionResult> GetAllTrackByArtist(int artistId, int? page, int? pageSize)
    {
        try
        {
            return Ok(await trackService.GetAllTrackByArtist(artistId, page, pageSize));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("filter/artist/{artistId}/solo")]
    public async Task<IActionResult> GetAllSoloTrackByArtist(int artistId, int? page, int? pageSize)
    {
        try
        {
            return Ok(await trackService.GetAllSoloTrackByArtist(artistId, page, pageSize));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("filter/album/{albumId}")]
    public async Task<IActionResult> GetAllTrackByAlbum(int albumId)
    {
        try
        {
            return Ok(await trackService.GetAllTrackByAlbum(albumId));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPost("add"), Authorize(Roles = "ARTIST")]
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

    [HttpPost("add/bulk"), Authorize(Roles = "ARTIST")]
    public async Task<IActionResult> CreateTracks([FromBody] List<CreateTrackDto> createTrack)
    {
        try
        {
            var tracks = await trackService.CreateTracks(createTrack);
            return StatusCode(StatusCodes.Status201Created, tracks);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPatch("{trackId}/album/{albumId}/add"), Authorize(Roles = "ARTIST")]
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

    [HttpPatch("{trackId}/album/remove"), Authorize(Roles = "ARTIST")]
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

    [HttpPatch("{trackId}/artist/{artistId}/add"), Authorize(Roles = "ARTIST")]
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

    [HttpPatch("{trackId}/artist/{artistId}/remove"), Authorize(Roles = "ARTIST")]
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

    [HttpPatch("{trackId}/genre/{genreId}/add"), Authorize(Roles = "ARTIST")]
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

    [HttpPatch("{trackId}/genre/{genreId}/remove"), Authorize(Roles = "ARTIST")]
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
