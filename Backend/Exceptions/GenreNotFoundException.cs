using System;

namespace MyMusic.Backend.Exceptions;

public class GenreNotFoundException : Exception
{
    public GenreNotFoundException() : base("Genre not found")
    { }
}
