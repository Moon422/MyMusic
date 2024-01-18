using System;

namespace MyMusic.Backend.Models;

public class ArtistUpgradeRequest
{
    public int Id { get; set; }
    public bool Approved { get; set; }

    public int RequestingProfileId { get; set; }
    public Profile RequestingProfile { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
