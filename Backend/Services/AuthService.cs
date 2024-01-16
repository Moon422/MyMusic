using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Services;

public interface IAuthService
{
    // string Test();
    Task<(LoginResponse, RefreshToken)> Login(LoginCredentials loginCredentials);
    Task<(LoginResponse, RefreshToken)> Register(Registration registration);
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

    public async Task<(LoginResponse, RefreshToken)> Login(LoginCredentials loginCredentials)
    {
        try
        {
            var profile = await dbcontext.Profiles.Include(p => p.User).FirstAsync(p => p.Email == loginCredentials.Email);

            if (BCrypt.Net.BCrypt.Verify(loginCredentials.Password, profile.User.Password))
            {
                return (new LoginResponse()
                {
                    Firstname = profile.Firstname,
                    Lastname = profile.Lastname,
                    Email = profile.Email,
                    Phonenumber = profile.Phonenumber,
                    JwtToken = CreateToken(profile)
                }, await GenerateRefreshToken(profile.User));
            }
            else
            {
                throw new LoginFailedException("Invalid credentials");
            }
        }
        catch (InvalidOperationException)
        {
            throw new LoginFailedException("Invalid credentials");
        }
    }

    public async Task<(LoginResponse, RefreshToken)> Register(Registration registration)
    {
        if (await ProfileExistsAsync(registration.Email, registration.Phonenumber))
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

        Console.WriteLine($"User ID: {user.Id}");

        return (new LoginResponse()
        {
            Firstname = profile.Firstname,
            Lastname = profile.Lastname,
            Email = profile.Email,
            Phonenumber = profile.Phonenumber,
            JwtToken = CreateToken(profile)
        }, await GenerateRefreshToken(user));
    }

    async Task<RefreshToken> GenerateRefreshToken(User user)
    {
        var currentDateTime = DateTime.UtcNow;

        RefreshToken refreshToken = new RefreshToken()
        {
            User = user,
            CreationTime = currentDateTime,
            UpdateTime = currentDateTime
        };

        await dbcontext.RefreshTokens.AddAsync(refreshToken);
        await dbcontext.SaveChangesAsync(true);

        return refreshToken;
    }

    async Task<bool> ProfileExistsAsync(string email, string phonenumber)
    {
        try
        {
            await dbcontext.Profiles.FirstAsync(p => p.Email == email || p.Phonenumber == phonenumber);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    string CreateToken(Profile profile)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, profile.Email),
            new Claim(ClaimTypes.MobilePhone, profile.Phonenumber),
            new Claim(ClaimTypes.Role, profile.ProfileTypes.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Secret").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
