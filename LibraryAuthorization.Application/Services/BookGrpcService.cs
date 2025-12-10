using AutoMapper;
using LibraryAuthorization.Application.Services.DTOs.BookModels;
using LibraryAuthorization.Application.Services.Interfaces;
using Librarymanagement;

namespace LibraryAuthorization.Application.Services;

public class BookGrpcService : IBookGrpcService
{
    private readonly BookService.BookServiceClient _bookServiceClient;
    private readonly IMapper _mapper;

    public BookGrpcService(
        BookService.BookServiceClient bookServiceClient,
        IMapper mapper
    )
    {
        _bookServiceClient = bookServiceClient;
        _mapper = mapper;
    }

    public async Task<BookDto> GetBookAsync(long Id)
    {
        var response = await _bookServiceClient.GetBookAsync(new BookGetRequest{BookId = Id});
        return _mapper.Map<BookDto>(response.Book);
    }
}
