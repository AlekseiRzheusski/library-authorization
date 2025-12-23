using LibraryAuthorization.Application.Services.DTOs.BorrowingModels;
using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Enums;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAuthorization.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowingController: ControllerBase
{
    private readonly IBorrowingGrpcService _borrowingGrpcService;
    public BorrowingController(
        IBorrowingGrpcService borrowingGrpcService
    )
    {
        _borrowingGrpcService = borrowingGrpcService;
    }

    [Authorize(Roles = "User")]
    [HttpPost("borrow")]
    public async Task<IActionResult> BorrowBook([FromBody]BorrowBookCommand command)
    {
        var userId = long.Parse((string)User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _borrowingGrpcService.BorrowBookAsync(command, userId);
        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpGet("borrowings")]
    public async Task<IActionResult> GetUserBorrowings(
        [FromQuery] BorrowingStatus status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 6)
    {
        var userId = long.Parse((string)User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _borrowingGrpcService.GetUserBorrowingsAsync(userId, status, pageNumber, pageSize);
        return Ok(result);
    }
}
