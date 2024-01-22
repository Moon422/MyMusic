using System;
using System.Collections.Generic;
using System.Linq;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Models;

public class Track
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TrackNumber { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Explicit { get; set; }
    public string TrackUrl { get; set; }

    public HashSet<Genre> Genres { get; set; }

    public int? AlbumId { get; set; }
    public Album Album { get; set; }

    public List<Artist> Artists { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }

    public ReadTrackDto ToReadDto()
    {
        return new ReadTrackDto
        {
            Id = this.Id,
            Name = this.Name,
            TrackNumber = this.TrackNumber,
            Duration = this.Duration,
            Explicit = this.Explicit,
            TrackUrl = this.TrackUrl,
            ArtistIds = this.Artists.Select(a => a.Id).ToList(),
            AlbumId = this.AlbumId,
            GenreIds = this.Genres.Select(g => g.Id).ToList()
        };
    }
}
