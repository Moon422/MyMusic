using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Services;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Registration registration)
    {
        await authService.Register(registration);
        return Ok();
    }
}
