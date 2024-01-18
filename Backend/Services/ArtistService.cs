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
    Task<ArtistResponse> Get();
    Task RequestProfileToArtistUpgrade();
    Task<IEnumerable<ProfileToArtistUpgradeRequestDto>> GetPendingProfileToArtistRequests();
    // Task<ArtistResponse> ApproveProfileToArtistUpgradeRequest();
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

    public async Task<ArtistResponse> Get()
    {
        #region 

        var email = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
        var artistProfile = await dbcontext.Artists
            .Include(a => a.Albums)
            .Include(a => a.Tracks)
            .Join(
                dbcontext.Profiles,
                a => a.ProfileId,
                p => p.Id,
                (artist, profile) => new { artist, profile })
            .FirstOrDefaultAsync(el => el.profile.Email == email);

        return new ArtistResponse
        {
            Id = artistProfile.artist.Id,
            ProfileId = artistProfile.artist.ProfileId,
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

    // public Task<ArtistResponse> ApproveProfileToArtistUpgradeRequest()
    // {
    //     #region



    //     #endregion
    // }
}
