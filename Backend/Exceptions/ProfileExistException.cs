using System;

namespace MyMusic.Backend.Exception;

public class ProfileExistException : System.Exception
{
    public ProfileExistException(string? message) : base(message)
    {
    }
}
