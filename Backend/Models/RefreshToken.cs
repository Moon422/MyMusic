using System;

namespace MyMusic.Backend.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool Active { get; set; } = true;

    public int UserId { get; set; }
    public User User { get; set; }


    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
