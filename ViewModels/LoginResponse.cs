using MyMusic.ViewModels.Enums;

namespace MyMusic.ViewModels;

public class LoginResponse
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Phonenumber { get; set; }
    public ProfileTypes ProfileType { get; set; }
    public string JwtToken { get; set; }
}
