using AutoMapper;
using LibraryAuthorization.Application.Services.DTOs.BookModels;
using Librarymanagement;

namespace LibraryAuthorization.Application.Mappers;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<BookResponse, BookDto>();
        CreateMap<BookListResponse, BookListDto>();
        
        CreateMap<SearchBookCommand, BookSearchRequest>();
    }
}
