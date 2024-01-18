using System;
using System.Collections.Generic;

namespace MyMusic.ViewModels;

public abstract class TrackDto
{
    public string Name { get; set; }
    public int TrackNumber { get; set; }
    public bool Explicit { get; set; }
    public string TrackUrl { get; set; }
    public List<int> GenreIds { get; set; }
    public int? AlbumId { get; set; }
    public List<int> ArtistIds { get; set; }
}

public class ReadTrackDto : TrackDto
{
    public int Id { get; set; }
    public TimeSpan Duration { get; set; }
}

public class CreateTrackDto : TrackDto
{
    public int Duration { get; set; }
}
