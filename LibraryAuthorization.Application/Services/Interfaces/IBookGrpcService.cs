using LibraryAuthorization.Application.Services.DTOs.BookModels;

namespace LibraryAuthorization.Application.Services.Interfaces;

public interface IBookGrpcService
{
    public Task<BookDto> GetBookAsync(long Id);
}
