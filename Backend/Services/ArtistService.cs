using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;
using MyMusic.ViewModels.Enums;

namespace MyMusic.Backend.Services;

public interface IArtistService
{
    Task<List<ArtistResponse>> GetArtists();
    Task<ArtistResponse> GetArtistById(int id);
    Task RequestProfileToArtistUpgrade();
    Task<IEnumerable<ProfileToArtistUpgradeRequestDto>> GetPendingProfileToArtistRequests();
    Task<ArtistResponse> ApprovePendingProfileToArtistRequest(int requestId);
    Task DisapprovePendingProfileToArtistRequest(int requestId);
}

public class ArtistService : IArtistService
{
    private readonly MusicDB dbcontext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public ArtistService(MusicDB dbcontext, IHttpContextAccessor httpContextAccessor)
    {
        this.dbcontext = dbcontext;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ArtistResponse>> GetArtists()
    {
        #region 

        var artists = await dbcontext.Artists
            .Include(a => a.Albums)
            .Include(a => a.Tracks)
            .Include(a => a.Profile)
            .Select(
                a => new ArtistResponse
                {
                    Id = a.Id,
                    Profile = new ProfileDto
                    {
                        Id = a.Profile.Id,
                        Firstname = a.Profile.Firstname,
                        Lastname = a.Profile.Lastname,
                        DateOfBirth = a.Profile.DateOfBirth,
                        Email = a.Profile.Email,
                        Phonenumber = a.Profile.Phonenumber,
                        ProfileType = a.Profile.ProfileType
                    },
                    AlbumIds = a.Albums.Select(a => a.Id).ToList(),
                    TrackIds = a.Tracks.Select(t => t.Id).ToList()
                }
            ).ToListAsync();

        return artists;

        #endregion
    }

    public async Task<ArtistResponse> GetArtistById(int id)
    {
        #region 

        var artistProfile = await dbcontext.Artists
            .Include(a => a.Albums)
            .Include(a => a.Tracks)
            .Join(
                dbcontext.Profiles,
                a => a.ProfileId,
                p => p.Id,
                (artist, profile) => new { artist, profile })
            .FirstOrDefaultAsync(el => el.artist.Id == id);

        return new ArtistResponse
        {
            Id = artistProfile.artist.Id,
            Profile = new ProfileDto
            {
                Id = artistProfile.profile.Id,
                Firstname = artistProfile.profile.Firstname,
                Lastname = artistProfile.profile.Lastname,
                DateOfBirth = artistProfile.profile.DateOfBirth,
                Email = artistProfile.profile.Email,
                Phonenumber = artistProfile.profile.Phonenumber,
                ProfileType = artistProfile.profile.ProfileType
            },
            AlbumIds = artistProfile.artist.Albums.Select(a => a.Id),
            TrackIds = artistProfile.artist.Tracks.Select(t => t.Id)
        };

        #endregion
    }

    public async Task RequestProfileToArtistUpgrade()
    {
        #region 

        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
        var profile = await dbcontext.Profiles.FirstOrDefaultAsync(p => p.Email == email);

        if (profile.ProfileType != ProfileTypes.LISTENER)
        {
            throw new ProfileCannotUpgradeToArtistException();
        }

        var request = await dbcontext.ArtistUpgradeRequests.FirstOrDefaultAsync(r => r.RequestingProfileId == profile.Id && r.Status == ArtistUpgradeRequestStatus.PENDING);

        if (request is not null)
        {
            throw new ProfileUpgradeToArtistRequestExistsException();
        }

        request = new ArtistUpgradeRequest
        {
            RequestingProfile = profile,
            CreationTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };

        await dbcontext.ArtistUpgradeRequests.AddAsync(request);
        await dbcontext.SaveChangesAsync(true);

        #endregion
    }

    public async Task<IEnumerable<ProfileToArtistUpgradeRequestDto>> GetPendingProfileToArtistRequests()
    {
        #region

        return await dbcontext.ArtistUpgradeRequests
            .Include(r => r.RequestingProfile)
            .Where(r => r.Status == ArtistUpgradeRequestStatus.PENDING)
            .Select(r => new ProfileToArtistUpgradeRequestDto
            {
                Id = r.Id,
                Firstname = r.RequestingProfile.Firstname,
                Lastname = r.RequestingProfile.Lastname,
                Email = r.RequestingProfile.Email,
                Phonenumber = r.RequestingProfile.Phonenumber,
                Status = r.Status
            })
            .ToListAsync();

        #endregion
    }

    public async Task<ArtistResponse> ApprovePendingProfileToArtistRequest(int requestId)
    {
        #region

        var request = await dbcontext.ArtistUpgradeRequests.Include(r => r.RequestingProfile).FirstOrDefaultAsync(r => r.Id == requestId);
        if (request is not null && request.Status == ArtistUpgradeRequestStatus.PENDING)
        {
            Artist artist = new Artist
            {
                Profile = request.RequestingProfile,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };

            request.RequestingProfile.ProfileType = ProfileTypes.ARTIST;
            request.Status = ArtistUpgradeRequestStatus.APPROVED;

            await dbcontext.AddAsync(artist);
            await dbcontext.SaveChangesAsync(true);

            return new ArtistResponse
            {
                Id = artist.Id,
                Profile = new ProfileDto
                {
                    Id = request.RequestingProfile.Id,
                    Firstname = request.RequestingProfile.Firstname,
                    Lastname = request.RequestingProfile.Lastname,
                    DateOfBirth = request.RequestingProfile.DateOfBirth,
                    Email = request.RequestingProfile.Email,
                    Phonenumber = request.RequestingProfile.Phonenumber,
                    ProfileType = request.RequestingProfile.ProfileType
                },
                AlbumIds = new List<int>(),
                TrackIds = new List<int>()
            };
        }
        else
        {
            throw new InvalidOperationException("Request not available");
        }

        #endregion
    }

    public async Task DisapprovePendingProfileToArtistRequest(int requestId)
    {
        #region

        var request = await dbcontext.ArtistUpgradeRequests.Include(r => r.RequestingProfile).FirstOrDefaultAsync(r => r.Id == requestId);
        if (request is not null && request.Status == ArtistUpgradeRequestStatus.PENDING)
        {
            request.Status = ArtistUpgradeRequestStatus.DISAPPROVED;
            await dbcontext.SaveChangesAsync(true);
        }
        else
        {
            throw new InvalidOperationException("Upgrade request not available");
        }

        #endregion
    }
}
