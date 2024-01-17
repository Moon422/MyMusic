using System;
using System.Collections.Generic;

namespace MyMusic.Backend.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Track> Tracks { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
