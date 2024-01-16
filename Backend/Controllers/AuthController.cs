using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
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

    [HttpGet("test"), Authorize]
    public async Task<IActionResult> Test()
    {
        return Ok(User?.Identity?.Name);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials loginCredentials)
    {
        try
        {
            return Ok(await authService.Login(loginCredentials));
        }
        catch (LoginFailedException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Registration registration)
    {
        try
        {
            LoginResponse respone = await authService.Register(registration);
            return Ok(respone);
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
