using System;

namespace MyMusic.Backend.Exceptions;

public class ArtistCannotBeRemovedException : Exception
{
    public ArtistCannotBeRemovedException() : base("Artist cannot be removed")
    {
    }
}
