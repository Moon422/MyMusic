using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Services;

public interface ITrackService
{
    Task<ReadTrackDto> GetTrackById(int id);
    Task<List<ReadTrackDto>> GetAllTrack();
    Task<ReadTrackDto> CreateTrack(CreateTrackDto createTrack);
    Task<ReadTrackDto> AddTrackToAlbum(int trackId, int albumId);
    Task<ReadTrackDto> RemoveTrackFromAlbum(int trackId);
    Task<ReadTrackDto> AddArtistToTrack(int trackId, int artistId);
    Task<ReadTrackDto> RemoveArtistfromTrack(int trackId, int artistId);
}

public class TrackService : ITrackService
{
    private readonly MusicDB dbcontext;

    public TrackService(MusicDB dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    public async Task<ReadTrackDto> GetTrackById(int id)
    {
        var track = await dbcontext.Tracks
            .Include(t => t.Album)
            .Include(t => t.Artists)
            .Include(t => t.Genres)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (track is null)
        {
            throw new TrackNotFoundException();
        }

        Console.WriteLine(track.Duration.Ticks);

        return new ReadTrackDto
        {
            Id = track.Id,
            Name = track.Name,
            TrackNumber = track.TrackNumber,
            Duration = track.Duration,
            Explicit = track.Explicit,
            TrackUrl = track.TrackUrl,
            ArtistIds = track.Artists.Select(a => a.Id).ToList(),
            AlbumId = track.AlbumId,
            GenreIds = track.Genres.Select(g => g.Id).ToList()
        };
    }

    public async Task<List<ReadTrackDto>> GetAllTrack()
    {
        var tracks = await dbcontext.Tracks
            .Include(t => t.Album)
            .Include(t => t.Artists)
            .Include(t => t.Genres)
            .Select(
                track => new ReadTrackDto
                {
                    Id = track.Id,
                    Name = track.Name,
                    TrackNumber = track.TrackNumber,
                    Duration = track.Duration,
                    Explicit = track.Explicit,
                    TrackUrl = track.TrackUrl,
                    ArtistIds = track.Artists.Select(a => a.Id).ToList(),
                    AlbumId = track.AlbumId,
                    GenreIds = track.Genres.Select(g => g.Id).ToList()
                }
            )
            .ToListAsync();
        return tracks;
    }

    public async Task<ReadTrackDto> CreateTrack(CreateTrackDto createTrack)
    {
        Album album = null;

        if (createTrack.AlbumId.HasValue)
        {
            album = await dbcontext.Albums.FindAsync(createTrack.AlbumId);
        }

        var artists = await dbcontext.Artists.Where(a => createTrack.ArtistIds.Contains(a.Id)).ToListAsync();
        var genres = await dbcontext.Genres.Where(g => createTrack.GenreIds.Contains(g.Id)).ToListAsync();

        Track track = new Track
        {
            Name = createTrack.Name,
            TrackNumber = createTrack.TrackNumber,
            Duration = new TimeSpan(0, 0, createTrack.Duration),
            Explicit = createTrack.Explicit,
            TrackUrl = createTrack.TrackUrl,
            Genres = genres,
            Album = album,
            Artists = artists,
            CreationTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        await dbcontext.AddAsync(track);
        await dbcontext.SaveChangesAsync(true);

        return new ReadTrackDto
        {
            Id = track.Id,
            Name = track.Name,
            TrackNumber = track.TrackNumber,
            Duration = track.Duration,
            Explicit = track.Explicit,
            TrackUrl = track.TrackUrl,
            ArtistIds = track.Artists.Select(a => a.Id).ToList(),
            AlbumId = track.AlbumId,
            GenreIds = track.Genres.Select(g => g.Id).ToList()
        };
    }

    public async Task<ReadTrackDto> AddTrackToAlbum(int trackId, int albumId)
    {
        if ((await dbcontext.Tracks.FindAsync(trackId)) is Track track)
        {
            if (track.Album is not null)
            {
                throw new TrackHasAlbumException();
            }

            if ((await dbcontext.Albums.FindAsync(albumId)) is Album album)
            {
                track.Album = album;
                await dbcontext.SaveChangesAsync(true);

                return track.ToReadDto();
            }
            else
            {
                throw new AlbumNotFoundException();
            }
        }
        else
        {
            throw new TrackNotFoundException();
        }
    }

    public async Task<ReadTrackDto> RemoveTrackFromAlbum(int trackId)
    {
        if ((await dbcontext.Tracks.FindAsync(trackId)) is Track track)
        {
            track.Album = null;
            await dbcontext.SaveChangesAsync(true);

            return track.ToReadDto();
        }
        else
        {
            throw new TrackNotFoundException();
        }
    }

    public async Task<ReadTrackDto> AddArtistToTrack(int trackId, int artistId)
    {
        if ((await dbcontext.Tracks.FindAsync(trackId)) is Track track)
        {
            if ((await dbcontext.Artists.FindAsync(artistId)) is Artist artist)
            {
                track.Artists.Add(artist);
                await dbcontext.SaveChangesAsync(true);

                return track.ToReadDto();
            }
            else
            {
                throw new ArtistNotFoundException();
            }
        }
        else
        {
            throw new TrackNotFoundException();
        }
    }

    public async Task<ReadTrackDto> RemoveArtistfromTrack(int trackId, int artistId)
    {
        if ((await dbcontext.Artists.FindAsync(artistId)) is Artist artist)
        {
            if ((await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == artistId && t.Artists.Contains(artist))) is Track track)
            {
                if (track.Artists.Count > 1)
                {
                    track.Artists.Remove(artist);
                    await dbcontext.SaveChangesAsync(true);

                    return track.ToReadDto();
                }
                else
                {
                    throw new ArtistCannotBeRemovedException();
                }
            }
            else
            {
                throw new TrackNotFoundException();
            }
        }
        else
        {
            throw new ArtistNotFoundException();
        }
    }
}
