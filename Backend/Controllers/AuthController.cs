using System;
using System.Security.Claims;
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
        // Console.WriteLine($"Local Time: {DateTime.Now}\tUTC Time: {DateTime.UtcNow}");

        // var email = User?.FindFirstValue(ClaimTypes.Email);
        // var phonenumber = User?.FindFirstValue(ClaimTypes.MobilePhone);

        return Ok(authService.Test());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials loginCredentials)
    {
        try
        {
            var loginResponse = await authService.Login(loginCredentials);
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
            var loginResponse = await authService.Register(registration);
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

    [HttpGet("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var loginResponse = await authService.RefreshToken();
            return Ok(loginResponse);
        }
        catch (RefreshTokenExpiredException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ProfileNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (RefreshTokenNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong. Please try againg");
        }
    }

    [HttpGet("logout"), Authorize]
    public async Task<IActionResult> Logout()
    {
        await authService.Logout();
        return NoContent();
    }
}
