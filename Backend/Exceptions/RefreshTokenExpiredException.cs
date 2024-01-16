using System;

namespace MyMusic.Backend.Exceptions;

public class RefreshTokenExpiredException : Exception
{
    public RefreshTokenExpiredException() : base("Refresh token expired.")
    {
    }
}
