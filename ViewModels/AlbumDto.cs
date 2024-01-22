using System;
using System.Collections.Generic;

namespace MyMusic.ViewModels;

public abstract class AlbumDto
{
    public string Name { get; set; }
    public List<int> ArtistIds { get; set; }
    public List<int> TrackIds { get; set; }
    public DateTime ReleaseDate { get; set; }
}

public class ReadAlbumDto : AlbumDto
{
    public int Id { get; set; }
}

public class CreateAlbumDto : AlbumDto
{ }
