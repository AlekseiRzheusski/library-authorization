using LibraryAuthorization.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAuthorization.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookGrpcService _bookGrpcService;

    public BookController(IBookGrpcService bookGrpcService)
    {
        _bookGrpcService = bookGrpcService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(long id)
    {
        var result = await _bookGrpcService.GetBookAsync(id);
        return Ok(result);  
    }
}
