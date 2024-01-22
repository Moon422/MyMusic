using System;

namespace MyMusic.Backend.Exceptions;

public class AlbumNotFoundException : NotFoundException
{
    public AlbumNotFoundException() : base("Album not found")
    { }
}
