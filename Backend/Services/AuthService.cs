using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Services;

public interface IAuthService
{
    IEnumerable Test();
    Task<LoginResponse> Login(LoginCredentials loginCredentials);
    Task<LoginResponse> Register(Registration registration);
    Task<LoginResponse> RefreshToken();
    Task Logout();
}

public class AuthService : IAuthService
{
    private readonly MusicDB dbcontext;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IConfiguration configuration;

    public AuthService(MusicDB dbcontext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        this.dbcontext = dbcontext;
        this.httpContextAccessor = httpContextAccessor;
        this.configuration = configuration;
    }

    public IEnumerable Test()
    {
        return new ArrayList();

        // var result = dbcontext.Users
        //     .Join(dbcontext.Profiles, u => u.Id, p => p.UserId, (u, p) => new { u, p })
        //     .Join(dbcontext.RefreshTokens, el => el.u.Id, r => r.UserId, (el, r) => new { el.u, el.p, r })
        //     .Select(
        //         el => new UserProfileDto
        //         {
        //             Profile = new ProfileDto
        //             {
        //                 Id = el.p.Id,
        //                 Firstname = el.p.Firstname,
        //                 Lastname = el.p.Lastname,
        //                 DateOfBirth = el.p.DateOfBirth,
        //                 Email = el.p.Email,
        //                 Phonenumber = el.p.Phonenumber,
        //                 ProfileType = (int)el.p.ProfileType,
        //                 UserId = el.p.UserId
        //             },
        //             User = new UserDto
        //             {
        //                 Id = el.u.Id,
        //                 Password = el.u.Password
        //             },
        //             RefreshTokenDto = new RefreshTokenDto
        //             {
        //                 Id = el.r.Id,
        //                 Token = el.r.Token,
        //                 ExpiryDate = el.r.ExpiryDate,
        //                 Active = el.r.Active,
        //                 UserId = el.r.UserId
        //             }
        //         }
        //     );
        // return result;

        // return configuration.GetSection("Secret").Value!;
    }

    public async Task<LoginResponse> Login(LoginCredentials loginCredentials)
    {
        try
        {
            var profile = await dbcontext.Profiles.Include(p => p.User).FirstAsync(p => p.Email == loginCredentials.Email);

            if (BCrypt.Net.BCrypt.Verify(loginCredentials.Password, profile.User.Password))
            {
                var refreshToken = await GenerateRefreshToken(profile.User);
                httpContextAccessor.HttpContext!.Response.Cookies.Append(
                    "refresh-token",
                    refreshToken.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
                );

                return new LoginResponse()
                {
                    Firstname = profile.Firstname,
                    Lastname = profile.Lastname,
                    Email = profile.Email,
                    Phonenumber = profile.Phonenumber,
                    JwtToken = CreateToken(profile)
                };
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

    public async Task<LoginResponse> Register(Registration registration)
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

        var refreshToken = await GenerateRefreshToken(profile.User);
        httpContextAccessor.HttpContext!.Response.Cookies.Append(
            "refresh-token",
            refreshToken.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            }
        );

        return new LoginResponse()
        {
            Firstname = profile.Firstname,
            Lastname = profile.Lastname,
            Email = profile.Email,
            Phonenumber = profile.Phonenumber,
            JwtToken = CreateToken(profile)
        };
    }

    public async Task<LoginResponse> RefreshToken()
    {
        var refreshKey = httpContextAccessor.HttpContext!.Request.Cookies["refresh-token"];

        try
        {
            var result = dbcontext.Users
            .Join(dbcontext.Profiles, u => u.Id, p => p.UserId, (u, p) => new { u, p })
            .Join(dbcontext.RefreshTokens, el => el.u.Id, r => r.UserId, (el, r) => new { el.u, el.p, r })
            .First(el => el.r.Token == refreshKey);

            RefreshToken oldRefreshToken = result.r;
            User user = result.u;
            Profile profile = result.p;

            if (oldRefreshToken.Active)
            {
                var timeToExpire = oldRefreshToken.ExpiryDate - DateTime.UtcNow;

                if (timeToExpire >= TimeSpan.Zero)
                {

                    if (timeToExpire.TotalDays < 1)
                    {
                        System.Console.WriteLine("Check Point 6");

                        var newRefreshToken = await GenerateRefreshToken(profile.User);
                        httpContextAccessor.HttpContext!.Response.Cookies.Append(
                            "refresh-token",
                            newRefreshToken.Token,
                            new CookieOptions
                            {
                                HttpOnly = true,
                                Expires = DateTimeOffset.UtcNow.AddDays(7)
                            }
                        );
                    }

                    return new LoginResponse()
                    {
                        Firstname = profile.Firstname,
                        Lastname = profile.Lastname,
                        Email = profile.Email,
                        Phonenumber = profile.Phonenumber,
                        JwtToken = CreateToken(profile)
                    };
                }
                else
                {
                    throw new RefreshTokenExpiredException();
                }

            }
            else
            {
                throw new RefreshTokenExpiredException();
            }
        }
        catch (InvalidOperationException)
        {
            throw new RefreshTokenNotFoundException();
        }
    }

    public async Task Logout()
    {
        var refreshKey = httpContextAccessor.HttpContext.Request.Cookies["refresh-token"];
        var refreshToken = await dbcontext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshKey);
        refreshToken.Active = false;
        await dbcontext.SaveChangesAsync();
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
            new Claim(ClaimTypes.Role, profile.ProfileType.ToString())
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
