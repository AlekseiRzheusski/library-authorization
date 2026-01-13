using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Moq;

using LibraryAuthorization.Application.Services.DTOs.BorrowingModels;
using LibraryAuthorization.Integration.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using LibraryAuthorization.Domain.Enums;

namespace LibraryAuthorization.Integration.Tests.Api;

[Collection("Borrowing collection")]
public class BorrowingControllerTests
{
    private readonly LibraryAuthorizationApiFactory _factory;

    public BorrowingControllerTests(LibraryAuthorizationApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task BorrowBook_IfRequestIsValid_ShouldReturnCreatedBorrowing()
    {
        var auth = _factory.Services.GetRequiredService<TestAuthContext>();
        auth.Claims.Clear();
        auth.Claims.Add(new Claim(ClaimTypes.NameIdentifier, "1"));
        auth.Claims.Add(new Claim(ClaimTypes.Role, "User"));

        var command = new BorrowBookCommand
        {
            BookId = 10
        };

        var expected = new BorrowingDto
        {
            BorrowingId = 10,
            BookId = 10,
            BookTitle = "Test book",
            UserId = 1,
            BorrowDate = "2026-01-02",
            DueDate = "2026-01-14",
            ReturnDate = "",
            Status = "Active"
        };

        _factory.BorrowingGrpcServiceMock
            .Setup(s => s.BorrowBookAsync(
                It.Is<BorrowBookCommand>(b => b.BookId == 10),
                1))
            .ReturnsAsync(expected);

        var response = await _factory.Client.PostAsJsonAsync(
            "/api/borrowing/borrow", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<BorrowingDto>();

        Assert.Equal(expected.BorrowingId, body!.BorrowingId);

        _factory.BorrowingGrpcServiceMock.Verify(s =>
            s.BorrowBookAsync(
                It.Is<BorrowBookCommand>(b => b.BookId == 10)
                , 1),
            Times.Once);
    }

    [Fact]
    public async Task GetUserBorrowings_IfBorrowingsExist_ShouldReturnListOfUserBorrowings()
    {
        var auth = _factory.Services.GetRequiredService<TestAuthContext>();
        auth.Claims.Clear();
        auth.Claims.Add(new Claim(ClaimTypes.NameIdentifier, "1"));
        auth.Claims.Add(new Claim(ClaimTypes.Role, "User"));

        var expected = new BorrowingListDto
        {
            Borrowings = new List<BorrowingDto>
            {
                new BorrowingDto
                {
                    BorrowingId = 100,
                    BookId = 10,
                    BookTitle = "Test book",
                    UserId = 1,
                    BorrowDate = "2026-01-02",
                    DueDate = "2026-01-14",
                    ReturnDate = "",
                    Status = "Active"
                },
                new BorrowingDto
                {
                    BorrowingId = 101,
                    BookId = 8,
                    BookTitle = "Test book 1",
                    UserId = 1,
                    BorrowDate = "2026-01-03",
                    DueDate = "2026-01-10",
                    ReturnDate = "",
                    Status = "Active"
                }
            },
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 2,
            NumberOfPages = 1
        };

        _factory.BorrowingGrpcServiceMock
            .Setup(s =>
                s.GetUserBorrowingsAsync(
                    It.IsAny<long>(),
                    It.IsAny<BorrowingStatus>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .ReturnsAsync(expected);

        var response = await _factory.Client
            .GetAsync("/api/borrowing/borrowings?status=Active&pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<BorrowingListDto>();

        Assert.Equal(expected.PageSize, body!.PageSize);
        Assert.Equal(
            expected.Borrowings.First<BorrowingDto>().BookTitle,
            body!.Borrowings.First<BorrowingDto>().BookTitle);

        _factory.BorrowingGrpcServiceMock.Verify(s =>
           s.GetUserBorrowingsAsync(1, BorrowingStatus.Active, 1, 10),
           Times.Once);
    }
}
