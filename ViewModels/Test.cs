using System;

namespace MyMusic.ViewModels;

public class ProfileDto
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phonenumber { get; set; }
    public int ProfileType { get; set; }

    public int UserId { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Password { get; set; }
}

public class RefreshTokenDto
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Active { get; set; } = true;

    public int UserId { get; set; }
}

public class UserProfileDto
{
    public ProfileDto Profile { get; set; }
    public UserDto User { get; set; }
    public RefreshTokenDto RefreshTokenDto { get; set; }
}
