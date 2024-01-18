using MyMusic.ViewModels.Enums;

namespace MyMusic.ViewModels;

public class ProfileToArtistUpgradeRequestDto
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Phonenumber { get; set; }
    public ArtistUpgradeRequestStatus Status { get; set; }
}
