using System;
using System.Collections.Generic;
using System.Linq;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Models;

public class Album
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }

    public List<Artist> Artists { get; set; }
    public List<Track> Tracks { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }

    public ReadAlbumDto ToReadDto()
    {
        return new ReadAlbumDto
        {
            Id = Id,
            Name = Name,
            ReleaseDate = ReleaseDate,
            ArtistIds = Artists.Select(a => a.Id).ToList(),
            TrackIds = Tracks.Select(t => t.Id).ToList()
        };
    }
}
