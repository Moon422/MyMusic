using Microsoft.EntityFrameworkCore;
using MyMusic.Backend.Models;

namespace MyMusic.Backend.Services;

public class MusicDB : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<ArtistUpgradeRequest> ArtistUpgradeRequests { get; set; }

    public MusicDB(DbContextOptions options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(
            options =>
            {
                options.HasKey(u => u.Id);
                options.Property(p => p.Password).HasMaxLength(60).IsFixedLength().IsRequired();
            }
        );

        modelBuilder.Entity<Profile>(
            options =>
            {
                options.HasKey(p => p.Id);
                options.Property(p => p.Firstname).HasMaxLength(255).IsRequired();
                options.Property(p => p.Lastname).HasMaxLength(255).IsRequired();
                options.Property(p => p.DateOfBirth).IsRequired();
                options.Property(p => p.Email).HasMaxLength(255).IsRequired();
                options.Property(p => p.Phonenumber).HasMaxLength(14).IsFixedLength().IsRequired();
                options.Property(p => p.ProfileType).HasDefaultValue(ProfileTypes.LISTENER).IsRequired();

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

        modelBuilder.Entity<RefreshToken>(
            options =>
            {
                options.HasKey(r => r.Id);

                options.Property(r => r.Token)
                    .HasMaxLength(32)
                    .IsFixedLength()
                    .IsRequired();
                options.Property(r => r.Active)
                    .HasDefaultValue(true)
                    .IsRequired();

                options.HasOne(r => r.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(r => r.UserId)
                    .IsRequired();

                options.HasIndex(r => r.Token);
            }
        );

        modelBuilder.Entity<Genre>(
            options =>
            {
                options.HasKey(g => g.Id);

                options.Property(g => g.Name)
                    .HasMaxLength(255)
                    .IsRequired();

                options.HasMany(g => g.Tracks)
                    .WithMany(t => t.Genres);
            }
        );

        modelBuilder.Entity<ArtistUpgradeRequest>(
            options =>
            {
                options.HasKey(a => a.Id);

                options.Property(a => a.Approved).HasDefaultValue(false).IsRequired();
            }
        )

        base.OnModelCreating(modelBuilder);
    }
}
