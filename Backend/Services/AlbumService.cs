using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Services;

public interface IAlbumService
{
    Task<ReadAlbumDto> GetAlbumById(int id);
    Task<List<ReadAlbumDto>> GetAllAlbum();
    Task<ReadAlbumDto> CreateAlbum(CreateAlbumDto albumDto);
    Task<ReadAlbumDto> AddTrack(int albumId, List<int> trackIds);
}

public class AlbumService : IAlbumService
{
    private readonly MusicDB dbcontext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AlbumService(MusicDB dbcontext, IHttpContextAccessor httpContextAccessor)
    {
        this.dbcontext = dbcontext;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<ReadAlbumDto> GetAlbumById(int id)
    {
        if ((await dbcontext.Albums.Include(a => a.Tracks).FirstOrDefaultAsync(a => a.Id == id)) is Album album)
        {
            var artists = album.Tracks.SelectMany(t => t.Artists).Distinct();
            var albumDto = album.ToReadDto(artists);

            return albumDto;
        }
        else
        {
            throw new NotFoundException($"Album with id {id} not found");
        }
    }

    public async Task<List<ReadAlbumDto>> GetAllAlbum()
    {
        var albumGroups = await dbcontext.Tracks
            .Include(t => t.Artists)
            .Include(t => t.Album)
            .Where(t => t.AlbumId.HasValue)
            .GroupBy(t => t.Album).ToListAsync();

        return albumGroups.Select(ag => ag.Key.ToReadDto(ag.SelectMany(t => t.Artists).Distinct())).ToList();
    }

    public async Task<ReadAlbumDto> CreateAlbum(CreateAlbumDto albumDto)
    {
        var tracks = await dbcontext.Tracks.Where(t => albumDto.TrackIds.Contains(t.Id)).ToListAsync();

        Album album = new Album
        {
            Name = albumDto.Name,
            ReleaseDate = albumDto.ReleaseDate,
            Tracks = tracks,
            CreationTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        await dbcontext.Albums.AddAsync(album);
        await dbcontext.SaveChangesAsync(true);

        return album.ToReadDto(null);
    }

    public async Task<ReadAlbumDto> AddTrack(int albumId, List<int> trackIds)
    {
        var tracks = await dbcontext.Tracks.Where(t => !t.AlbumId.HasValue && trackIds.Contains(t.Id)).ToListAsync();

        if (tracks.Count <= 0)
        {
            throw new NotFoundException("No tracks found with the following ID or without any album");
        }

        if ((await dbcontext.Albums.Include(a => a.Tracks).FirstOrDefaultAsync(a => a.Id == albumId)) is Album album)
        {
            album.Tracks.AddRange(tracks);
            await dbcontext.SaveChangesAsync();

            return album.ToReadDto(album.Tracks.SelectMany(t => t.Artists));
        }
        else
        {
            throw new NotFoundException("Album not found");
        }
    }
}
