using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyMusic.Backend.Services;
using Swashbuckle.AspNetCore.Filters;

namespace MyMusic.Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        string connectionString = builder.Configuration.GetConnectionString("mysql")!;
        MySqlServerVersion mySqlServerVersion = new MySqlServerVersion(new Version(8, 0, 35));

        builder.Services.AddDbContext<MusicDB>(
            option => option
                .UseMySql(connectionString, mySqlServerVersion)
        // .LogTo(Console.WriteLine, LogLevel.Information)
        // .EnableDetailedErrors()
        );

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IArtistService, ArtistService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<ITrackService, TrackService>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(
            options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            }
        );
        builder.Services.AddAuthentication().AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration.GetSection("Secret").Value!
                    ))
                };
            }
        );

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
