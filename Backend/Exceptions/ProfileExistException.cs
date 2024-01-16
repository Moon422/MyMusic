using System;

namespace MyMusic.Backend.Exceptions;

public class ProfileExistException : Exception
{
    public ProfileExistException(string? message) : base(message)
    {
    }
}
