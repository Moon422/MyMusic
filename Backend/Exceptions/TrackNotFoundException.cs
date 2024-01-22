namespace MyMusic.Backend.Exceptions;

public class TrackNotFoundException : NotFoundException
{
    public TrackNotFoundException() : base("Track not found")
    { }

    public TrackNotFoundException(string message) : base(message)
    { }
}
