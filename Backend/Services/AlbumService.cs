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
        if ((await dbcontext.Albums.FirstOrDefaultAsync(a => a.Id == id)) is Album album)
        {
            return album.ToReadDto();
        }
        else
        {
            throw new NotFoundException($"Album with id {id} not found");
        }
    }

    public async Task<List<ReadAlbumDto>> GetAllAlbum()
    {
        return await dbcontext.Albums
            .Include(a => a.Tracks)
            .Include(a => a.Artists)
            .Select(a => a.ToReadDto())
            .ToListAsync();
    }

    public async Task<ReadAlbumDto> CreateAlbum(CreateAlbumDto albumDto)
    {
        var artists = await dbcontext.Artists.Where(a => albumDto.ArtistIds.Contains(a.Id)).ToListAsync();
        var tracks = await dbcontext.Tracks.Where(t => albumDto.TrackIds.Contains(t.Id)).ToListAsync();

        Album album = new Album
        {
            Name = albumDto.Name,
            ReleaseDate = albumDto.ReleaseDate,
            Artists = artists,
            Tracks = tracks,
            CreationTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        await dbcontext.Albums.AddAsync(album);
        await dbcontext.SaveChangesAsync(true);

        return album.ToReadDto();

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
            foreach (var track in tracks)
            {
                if (!album.Tracks.Contains(track))
                {
                    album.Tracks.Add(track);
                }
            }

            await dbcontext.SaveChangesAsync();

            return album.ToReadDto();
        }
        else
        {
            throw new NotFoundException("Album not found");
        }
    }
}
