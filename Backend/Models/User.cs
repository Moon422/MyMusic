using System;

namespace MyMusic.Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Password { get; set; }

    public Profile Profile { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
