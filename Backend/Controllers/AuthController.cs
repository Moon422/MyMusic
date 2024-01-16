using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Models;
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
        // return Ok(User?.Identity?.Name);
        var email = User?.FindFirstValue(ClaimTypes.Email);
        var phonenumber = User?.FindFirstValue(ClaimTypes.MobilePhone);

        return Ok(new { email, phonenumber });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials loginCredentials)
    {
        try
        {
            var (loginResponse, refreshToken) = await authService.Login(loginCredentials);

            HttpContext.Response.Cookies.Append(
                "refresh-token",
                refreshToken.Token,
                new CookieOptions
                {
                    HttpOnly = true
                }
            );

            return Ok(loginResponse);
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
            var (loginResponse, refreshToken) = await authService.Register(registration);

            HttpContext.Response.Cookies.Append(
                "refresh-token",
                refreshToken.Token,
                new CookieOptions
                {
                    HttpOnly = true
                }
            );

            return Ok(loginResponse);
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
