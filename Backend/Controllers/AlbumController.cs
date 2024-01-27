using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Services;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumController : ControllerBase
{
    private readonly IAlbumService albumService;

    public AlbumController(IAlbumService albumService)
    {
        this.albumService = albumService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlbumById(int id)
    {
        try
        {
            var album = await albumService.GetAlbumById(id);
            return Ok(album);
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

    [HttpGet]
    public async Task<IActionResult> GetAllAlbum()
    {
        try
        {
            var albums = await albumService.GetAllAlbum();
            return Ok(albums);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("filter/artist/{artistId}")]
    public async Task<IActionResult> GetAllAlbum(int artistId)
    {
        try
        {
            var albums = await albumService.GetAlbumsByArtist(artistId);
            return Ok(albums);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPost, Authorize(Roles = "ARTIST")]
    public async Task<IActionResult> CreateAlbum([FromBody] CreateAlbumDto albumDto)
    {
        try
        {
            var album = await albumService.CreateAlbum(albumDto);
            return Ok(album);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPatch("{albumId}/track/add"), Authorize(Roles = "ARTIST")]
    public async Task<IActionResult> AddTrack(int albumId, [FromBody] List<int> trackIds)
    {
        try
        {
            var album = await albumService.AddTrack(albumId, trackIds);
            return Ok(album);
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
