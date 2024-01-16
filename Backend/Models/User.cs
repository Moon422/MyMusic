using System;
using System.Collections.Generic;

namespace MyMusic.Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Password { get; set; }

    public Profile Profile { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
