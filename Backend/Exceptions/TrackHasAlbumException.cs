using System;

namespace MyMusic.Backend.Exceptions;

public class TrackHasAlbumException : Exception
{
    public TrackHasAlbumException() : base("Track already has an album")
    { }
}
