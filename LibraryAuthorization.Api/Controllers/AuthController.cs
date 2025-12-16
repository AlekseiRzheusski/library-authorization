using LibraryAuthorization.Application.DTOs.AuthModels;
using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Entities;
using LibraryAuthorization.Infrastructure.Repositories.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace LibraryAuthorization.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    //TODO: move repository, JwtService and UserManager usage to the separate service
    private readonly UserManager<LibraryUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public AuthController(
        UserManager<LibraryUser> userManager,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
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

        var tokens = await _jwtService.GenerateJwtToken(user);
        return Ok(new { tokens });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var token = await _refreshTokenRepository.FindDetailedEntityByToken(command.RefreshToken);
        if (token == null || token.ExpiresAt < DateTime.UtcNow)
            return Unauthorized();

        token.IsRevoked = true;

        var tokens = await _jwtService.GenerateJwtToken(token.User);

        await _refreshTokenRepository.SaveAsync();
        return Ok(tokens);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenCommand command)
    {
        var token = await _refreshTokenRepository
            .FindFirstOrDefault(t => t.Token == command.RefreshToken && t.IsRevoked == false);

        if (token is null)
        {
            return NotFound();
        }

        token.IsRevoked = true;

        await _refreshTokenRepository.SaveAsync();
        return Ok();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = long.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var tokens = await _refreshTokenRepository
            .FindAndAddToContextAsync(t => t.UserId == userId && t.IsRevoked == false);

        if (tokens.Count() == 0)
        {
            return NotFound();
        }

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _refreshTokenRepository.SaveAsync();
        return Ok();
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
