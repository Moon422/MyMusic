using System;

namespace MyMusic.Backend.Exceptions;

public class LoginFailedException : Exception
{
    public LoginFailedException(string? message) : base(message)
    {
    }
}
