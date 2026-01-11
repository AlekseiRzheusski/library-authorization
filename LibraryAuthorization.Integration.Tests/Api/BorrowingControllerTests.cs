using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Moq;

using LibraryAuthorization.Application.Services.DTOs.BorrowingModels;
using LibraryAuthorization.Integration.Tests.Fixtures;

using Microsoft.Extensions.DependencyInjection;

namespace LibraryAuthorization.Integration.Tests.Api;

public class BorrowingControllerTests : IClassFixture<LibraryAuthorizationApiFactory>
{
    private readonly LibraryAuthorizationApiFactory _factory;

    public BorrowingControllerTests(LibraryAuthorizationApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task BorrowBook_IfRequestIsValid_ShouldRedurnCreatedBorrowing()
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
            BorrowingId = 100,
            BookId = 10
        };

        _factory.BorrowingServiceMock
            .Setup(s => s.BorrowBookAsync(
                It.IsAny<BorrowBookCommand>(),
                1))
            .ReturnsAsync(new BorrowingDto
            {
                BorrowingId = 100,
                BookId = 10
            });

        var response = await _factory.Client.PostAsJsonAsync(
            "/api/borrowing/borrow", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<BorrowingDto>();

        Assert.Equal(expected.BorrowingId, body!.BorrowingId);
    }
}
