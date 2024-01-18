using System;
using MyMusic.ViewModels.Enums;

namespace MyMusic.ViewModels;

public class ProfileDto
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phonenumber { get; set; }
    public ProfileTypes ProfileType { get; set; }
}
