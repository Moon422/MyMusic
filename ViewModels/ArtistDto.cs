using System.Collections.Generic;

namespace MyMusic.ViewModels;

public class ArtistResponse
{
    public int Id { get; set; }
    public ProfileDto Profile { get; set; }
    public IEnumerable<int> AlbumIds { get; set; }
    public IEnumerable<int> TrackIds { get; set; }
}
