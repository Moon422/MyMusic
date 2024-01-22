using System;
using System.Collections.Generic;
using System.Linq;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Models;

public class Artist
{
    public int Id { get; set; }

    public int ProfileId { get; set; }
    public Profile Profile { get; set; }

    // public HashSet<Album> Albums { get; set; }
    public List<Track> Tracks { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }

    public ArtistDto ToDto()
    {
        var albumIds = Tracks.Select(t => t.AlbumId ?? 0);

        return new ArtistDto
        {
            Id = Id,
            Profile = Profile.ToDto(),
            AlbumIds = albumIds,
            TrackIds = Tracks.Select(t => t.Id)
        };
    }
}
