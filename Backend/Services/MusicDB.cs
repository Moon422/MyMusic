using Microsoft.EntityFrameworkCore;
using MyMusic.Backend.Models;

namespace MyMusic.Backend.Services;

public class MusicDB : DbContext
{
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<User> Users { get; set; }

    public MusicDB(DbContextOptions options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(
            options =>
            {
                options.HasKey(p => p.Id);
                options.Property(p => p.Firstname).HasMaxLength(255).IsRequired();
                options.Property(p => p.Lastname).HasMaxLength(255).IsRequired();
                options.Property(p => p.DateOfBirth).IsRequired();
                options.Property(p => p.Email).HasMaxLength(255).IsRequired();
                options.Property(p => p.Phonenumber).HasMaxLength(14).IsFixedLength().IsRequired();
                options.Property(p => p.ProfileTypes).HasDefaultValue(ProfileTypes.LISTENER).IsRequired();
                options.HasOne(p => p.User)
                    .WithOne(u => u.Profile)
                    .HasForeignKey<Profile>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                options.HasIndex(p => p.Email)
                    .IsUnique();
                options.HasIndex(p => p.Phonenumber)
                    .IsUnique();
            }
        );

        modelBuilder.Entity<User>(
            options =>
            {
                options.HasKey(u => u.Id);
                options.Property(p => p.Password).HasMaxLength(60).IsFixedLength().IsRequired();
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}
