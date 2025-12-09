using LibraryAuthorization.Application.DTOs.AuthModels;
using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAuthorization.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly UserManager<LibraryUser> _userManager;
    private readonly IJwtService _jwtService;
    public AuthController(
        UserManager<LibraryUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var user = new LibraryUser { UserName = command.Username };
        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        return Ok("Registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var user = await _userManager.FindByNameAsync(command.Username);
        if (user == null)
            return Unauthorized();

        if (!await _userManager.CheckPasswordAsync(user, command.Password))
            return Unauthorized();

        var token = await _jwtService.GenerateJwtToken(user);
        return Ok(new { token });
    }

    [HttpGet]
    public IActionResult Test()
    {
        return Ok("You good?");
    }

    [Authorize]
    [HttpGet("user")]
    public IActionResult TestAuth()
    {
        return Ok(User.Identity!.Name);
    }
} 
