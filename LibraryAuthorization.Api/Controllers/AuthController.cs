using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly UserManager<LibraryUser> _userManager;
    public AuthController(UserManager<LibraryUser> userManager)
    {
        _userManager = userManager;
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

    [HttpGet]
    public IActionResult Test()
    {
        return Ok("You good?");
    }
} 
