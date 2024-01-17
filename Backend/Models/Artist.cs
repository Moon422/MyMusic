using System;
using System.Collections.Generic;

namespace MyMusic.Backend.Models;

public class Artist
{
    public int Id { get; set; }

    public int ProfileId { get; set; }
    public Profile Profile { get; set; }

    public List<Album> Albums { get; set; }
    public List<Track> Tracks { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
