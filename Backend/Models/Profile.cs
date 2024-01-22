using System;
using System.Collections.Generic;
using MyMusic.ViewModels;
using MyMusic.ViewModels.Enums;

namespace MyMusic.Backend.Models;

public class Profile
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phonenumber { get; set; }
    public ProfileTypes ProfileType { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime UpdateTime { get; set; }

    public ProfileDto ToDto()
    {
        return new ProfileDto
        {
            Id = Id,
            Firstname = Firstname,
            Lastname = Lastname,
            DateOfBirth = DateOfBirth,
            Email = Email,
            Phonenumber = Phonenumber,
            ProfileType = ProfileType
        };
    }

    public override string ToString()
    {
        return $"ID: {Id}\nFirstname: {Firstname}\nLastname: {Lastname}";
    }
}
