namespace MyMusic.Backend.Exceptions;

public class ProfileNotFoundException : NotFoundException
{
    public ProfileNotFoundException() : base("Profile not found.")
    { }
}
