using LibraryAuthorization.Application.Services.DTOs.BookModels;
using LibraryAuthorization.Integration.Tests.Fixtures;

using System.Net.Http.Json;
using Moq;
using System.Net;
using Grpc.Core;
using System.Text.Json;

namespace LibraryAuthorization.Integration.Tests.Api;

public class BookControllerTests : IClassFixture<LibraryAuthorizationApiFactory>
{
    private readonly LibraryAuthorizationApiFactory _factory;
    public BookControllerTests(LibraryAuthorizationApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetBook_WhenBookIdExists_ShouldReturnBookDto()
    {
        var expected = new BookDto { BookId = 1, Title = "Test Book" };
        _factory.BookGrpcServiceMock
                .Setup(s => s.GetBookAsync(1))
                .ReturnsAsync(expected);

        var response = await _factory.Client.GetAsync("/api/book/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.Equal(expected.BookId, body!.BookId);
    }

    [Fact]
    public async Task GetBook_WhenServiceRaisesNotFoundRpcException_ShouldReturn404()
    {
        var exMessage = "Book with ID 100 does not exist";
        _factory.BookGrpcServiceMock
            .Setup(s => s.GetBookAsync(100))
            .ThrowsAsync(new RpcException(new Status(StatusCode.NotFound, exMessage)));

        var response = await _factory.Client.GetAsync("/api/book/100");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

        Assert.Equal(exMessage, body["error"].ToString());
        Assert.Equal("NotFound", body["grpcCode"].ToString());
    }
}
