namespace MyMusic.Backend.Exceptions;

public class ArtistNotFoundException : NotFoundException
{
    public ArtistNotFoundException() : base("Artist not found")
    { }
}
