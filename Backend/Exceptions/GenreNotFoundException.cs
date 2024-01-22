namespace MyMusic.Backend.Exceptions;

public class GenreNotFoundException : NotFoundException
{
    public GenreNotFoundException() : base("Genre not found")
    { }
}
