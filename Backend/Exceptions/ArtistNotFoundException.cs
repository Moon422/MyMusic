using System;

namespace MyMusic.Backend.Exceptions;

public class ArtistNotFoundException : Exception
{
    public ArtistNotFoundException() : base("Artist not found")
    { }
}
