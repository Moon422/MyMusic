using System;

namespace MyMusic.Backend.Exceptions;

public class AlbumNotFoundException : Exception
{
    public AlbumNotFoundException() : base("Album not found")
    { }
}
