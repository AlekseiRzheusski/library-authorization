using LibraryAuthorization.Application.Services.DTOs.BookModels;
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

    [HttpGet("list")]
    public async Task<IActionResult> GetBooks(
        [FromBody] SearchBookCommand? command,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 6)
    {
        command ??= new SearchBookCommand();
        var result = await _bookGrpcService.GetBooksAsync(command, pageNumber, pageSize);
        return Ok(result);
    }
}
