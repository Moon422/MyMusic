using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    Task<List<ReadTrackDto>> CreateTracks(List<CreateTrackDto> createTracks);
    Task<ReadTrackDto> AddTrackToAlbum(int trackId, int albumId, bool adminMode = false);
    Task<ReadTrackDto> RemoveTrackFromAlbum(int trackId, bool adminMode = false);
    Task<ReadTrackDto> AddArtistToTrack(int trackId, int artistId, bool adminMode = false);
    Task<ReadTrackDto> RemoveArtistfromTrack(int trackId, int artistId, bool adminMode = false);
    Task<ReadTrackDto> AddGenreToTrack(int trackId, int genreId, bool adminMode = false);
    Task<ReadTrackDto> RemoveGenreFromTrack(int trackId, int genreId, bool adminMode = false);
}

public class TrackService : ITrackService
{
    private readonly MusicDB dbcontext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public TrackService(MusicDB dbcontext, IHttpContextAccessor httpContextAccessor)
    {
        this.dbcontext = dbcontext;
        this.httpContextAccessor = httpContextAccessor;
    }

    async Task<Track> CreateTrackFromDto(CreateTrackDto createTrack, Album album)
    {
        var artists = await dbcontext.Artists.Where(a => createTrack.ArtistIds.Contains(a.Id)).ToListAsync();
        var genres = await dbcontext.Genres.Where(g => createTrack.GenreIds.Contains(g.Id)).ToListAsync();

        return new Track
        {
            Name = createTrack.Name,
            TrackNumber = createTrack.TrackNumber,
            Duration = new TimeSpan(0, 0, createTrack.Duration),
            Explicit = createTrack.Explicit,
            TrackUrl = createTrack.TrackUrl,
            Genres = new HashSet<Genre>(genres),
            Album = album,
            Artists = artists,
            CreationTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };
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
            .Select(track => track.ToReadDto())
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

        Track track = await CreateTrackFromDto(createTrack, album);

        await dbcontext.AddAsync(track);
        await dbcontext.SaveChangesAsync(true);

        return track.ToReadDto();
    }

    public async Task<List<ReadTrackDto>> CreateTracks(List<CreateTrackDto> createTracks)
    {
        List<Track> tracks = new List<Track>();

        foreach (var createTrack in createTracks)
        {
            Album album = null;

            if (createTrack.AlbumId.HasValue)
            {
                album = await dbcontext.Albums.FindAsync(createTrack.AlbumId);
            }

            Track track = await CreateTrackFromDto(createTrack, album);

            await dbcontext.AddAsync(track);
            tracks.Add(track);
        }

        await dbcontext.SaveChangesAsync(true);

        return tracks.Select(t => t.ToReadDto()).ToList();
    }

    public async Task<ReadTrackDto> AddTrackToAlbum(int trackId, int albumId, bool adminMode = false)
    {
        if ((await dbcontext.Albums.FindAsync(albumId)) is Album album)
        {
            Track track;

            if (adminMode)
            {
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
            }
            else
            {
                string email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                Artist artist = await dbcontext.Artists.Include(a => a.Profile).FirstOrDefaultAsync(a => a.Profile.Email == email);
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId && t.Artists.Contains(artist));
            }

            if (track is not null)
            {
                track.Album = album;
                await dbcontext.SaveChangesAsync(true);

                return track.ToReadDto();
            }
            else
            {
                throw new TrackNotFoundException($"Track with id {trackId} not found");
            }
        }
        else
        {
            throw new AlbumNotFoundException();
        }
    }

    public async Task<ReadTrackDto> RemoveTrackFromAlbum(int trackId, bool adminMode = false)
    {
        Track track;

        if (adminMode)
        {
            track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
        }
        else
        {
            string email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            Artist artist = await dbcontext.Artists.Include(a => a.Profile).FirstOrDefaultAsync(a => a.Profile.Email == email);
            track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId && t.Artists.Contains(artist));
        }

        if (track is not null)
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

    public async Task<ReadTrackDto> AddArtistToTrack(int trackId, int artistId, bool adminMode = false)
    {
        if ((await dbcontext.Artists.FindAsync(artistId)) is Artist artist)
        {
            Track track;

            if (adminMode)
            {
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
            }
            else
            {
                string email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                Artist a = await dbcontext.Artists.Include(a => a.Profile).FirstOrDefaultAsync(a => a.Profile.Email == email);
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId && t.Artists.Contains(a));
            }

            if (track is not null)
            {
                track.Artists.Add(artist);
                await dbcontext.SaveChangesAsync(true);
                return track.ToReadDto();
            }
            else
            {
                throw new TrackNotFoundException($"Track with id {trackId} not found, or track contains the artist with id {artistId}");
            }
        }
        else
        {
            throw new ArtistNotFoundException();
        }
    }

    public async Task<ReadTrackDto> RemoveArtistfromTrack(int trackId, int artistId, bool adminMode = false)
    {
        if ((await dbcontext.Artists.FindAsync(artistId)) is Artist artist)
        {
            Track track;

            if (adminMode)
            {
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
            }
            else
            {
                string email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                Artist a = await dbcontext.Artists.Include(a => a.Profile).FirstOrDefaultAsync(a => a.Profile.Email == email);
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId && t.Artists.Contains(a));
            }

            if (track is not null)
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

    public async Task<ReadTrackDto> AddGenreToTrack(int trackId, int genreId, bool adminMode = false)
    {
        if ((await dbcontext.Genres.FindAsync(genreId)) is Genre genre)
        {
            Track track;

            if (adminMode)
            {
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
            }
            else
            {
                string email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                Artist artist = await dbcontext.Artists.Include(a => a.Profile).FirstOrDefaultAsync(a => a.Profile.Email == email);
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId && t.Artists.Contains(artist));
            }

            if (track is not null)
            {
                track.Genres.Add(genre);
                await dbcontext.SaveChangesAsync();
                return track.ToReadDto();
            }
            else
            {
                throw new TrackNotFoundException($"Track with id {trackId} not found, or track contains the genre with id {genreId}");
            }
        }
        else
        {
            throw new GenreNotFoundException();
        }
    }

    public async Task<ReadTrackDto> RemoveGenreFromTrack(int trackId, int genreId, bool adminMode = false)
    {
        if ((await dbcontext.Genres.FindAsync(genreId)) is Genre genre)
        {
            Track track;

            if (adminMode)
            {
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
            }
            else
            {
                string email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                Artist artist = await dbcontext.Artists.Include(a => a.Profile).FirstOrDefaultAsync(a => a.Profile.Email == email);
                track = await dbcontext.Tracks.FirstOrDefaultAsync(t => t.Id == trackId && t.Artists.Contains(artist));
            }

            if (track is not null)
            {
                track.Genres.Remove(genre);
                await dbcontext.SaveChangesAsync();
                return track.ToReadDto();
            }
            else
            {
                throw new TrackNotFoundException();
            }
        }
        else
        {
            throw new GenreNotFoundException();
        }
    }
}
