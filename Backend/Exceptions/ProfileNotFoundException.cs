using System;

namespace MyMusic.Backend.Exceptions;

public class ProfileNotFoundException : Exception
{
    public ProfileNotFoundException() : base("Profile not found.")
    { }
}
