using System.Threading.Tasks;
using MyMusic.Backend.Models;
using BCrypt;
using MyMusic.ViewModels;
using System;

namespace MyMusic.Backend.Services;

public interface IAuthService
{
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

        Console.WriteLine(user);
    }
}
