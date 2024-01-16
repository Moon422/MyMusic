using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exception;
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

    // [HttpGet("test")]
    // public async Task<IActionResult> Test()
    // {
    //     return Ok(authService.Test());
    // }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Registration registration)
    {
        try
        {
            await authService.Register(registration);
            return Ok();
        }
        catch (ProfileExistException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }
}
