namespace MyMusic.Backend.Exceptions;

public class RefreshTokenNotFoundException : NotFoundException
{
    public RefreshTokenNotFoundException() : base("Refresh token not found.")
    { }
}
