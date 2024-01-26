using System.ComponentModel.DataAnnotations;

namespace MyMusic.ViewModels;

public class LoginCredentials
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
