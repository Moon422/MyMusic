using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyMusic.Backend.Exception;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Services;

public interface IAuthService
{
    // string Test();
    Task Register(Registration registration);
}

public class AuthService : IAuthService
{
    MusicDB dbcontext;
    IConfiguration configuration;

    public AuthService(MusicDB dbcontext, IConfiguration configuration)
    {
        this.dbcontext = dbcontext;
        this.configuration = configuration;
    }

    public string Test()
    {
        return configuration.GetSection("Secret").Value!;
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Secret").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        return "";
    }
}
