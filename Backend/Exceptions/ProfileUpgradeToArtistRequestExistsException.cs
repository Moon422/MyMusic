using System;

namespace MyMusic.Backend.Exceptions;

public class ProfileUpgradeToArtistRequestExistsException : Exception
{
    public ProfileUpgradeToArtistRequestExistsException() : base("Profile upgrade to artist request already made")
    { }
}
