using System;

namespace MyMusic.Backend.Models;

public class Artist
{
    public int Id { get; set; }



    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

public class Album
{
    public int Id { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

public class Track
{
    public int Id { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
