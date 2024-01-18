using System;
using System.Collections.Generic;

namespace MyMusic.Backend.Models;

public class Track
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TrackNumber { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Explicit { get; set; }
    public string TrackUrl { get; set; }

    public List<Genre> Genres { get; set; }

    public int? AlbumId { get; set; }
    public Album Album { get; set; }

    public List<Artist> Artists { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
