using System;

namespace MyMusic.Backend.Exceptions;

public class ProfileCannotUpgradeToArtistException : Exception
{
    public ProfileCannotUpgradeToArtistException() : base("Profile cannot be upgraded to artist")
    {
    }
}
