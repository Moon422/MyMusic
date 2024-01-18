using System;

namespace MyMusic.Backend.Exceptions;

public class TrackNotFoundException : Exception
{
    public TrackNotFoundException() : base("Track not found")
    { }
}
