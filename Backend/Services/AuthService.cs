using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMusic.Backend.Exception;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Services;

public interface IAuthService
{
    // IEnumerable Test();
    Task Register(Registration registration);
}

public class AuthService : IAuthService
{
    MusicDB dbcontext;

    public AuthService(MusicDB dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    public async Task Register(Registration registration)
    {
        if ((await dbcontext.Profiles.FirstOrDefaultAsync(profile => profile.Email == registration.Email || profile.Phonenumber == registration.Phonenumber)) != null)
        {
            throw new ProfileExistException("Profile with the provided email or phonenumber already exists.");
        }

        var currentDateTime = DateTime.UtcNow;
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registration.Password);

        User user = new User()
        {
            Password = hashedPassword,
            CreationTime = currentDateTime,
            UpdateTime = currentDateTime
        };

        Profile profile = new Profile()
        {
            Firstname = registration.FirstName,
            Lastname = registration.Lastname,
            DateOfBirth = registration.DateOfBirth,
            Email = registration.Email,
            Phonenumber = registration.Phonenumber,
            User = user,
            CreationTime = currentDateTime,
            UpdateTime = currentDateTime
        };

        await dbcontext.Users.AddAsync(user);
        await dbcontext.Profiles.AddAsync(profile);
        await dbcontext.SaveChangesAsync(true);
    }

    string CreateToken(Profile profile)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, profile.Email),
            new Claim(ClaimTypes.MobilePhone, profile.Phonenumber)
        };

        // var key = 

        return "";
    }

    public IEnumerable Test()
    {
        return dbcontext.Users.Select(u => new { Password = u.Password, Length = u.Password.Length });
    }
}
