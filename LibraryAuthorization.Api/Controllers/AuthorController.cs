using LibraryAuthorization.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorGrpcService _authorGrpcService;

    public AuthorController(
        IAuthorGrpcService authorGrpcService
    )
    {
        _authorGrpcService = authorGrpcService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(long id)
    {
        var result = await _authorGrpcService.GetAuthorAsync(id);
        return Ok(result);
    }
}
