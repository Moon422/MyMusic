using System;

namespace MyMusic.Backend.Exceptions;

public class RefreshTokenNotFoundException : Exception
{
    public RefreshTokenNotFoundException() : base("Refresh token not found.")
    { }
}
