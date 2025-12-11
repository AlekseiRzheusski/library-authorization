using AutoMapper;
using LibraryAuthorization.Application.Services.DTOs.AuthorModels;
using LibraryAuthorization.Application.Services.Interfaces;
using Librarymanagement;

namespace LibraryAuthorization.Application.Services;

public class AuthorGrpcService : IAuthorGrpcService
{
    private readonly AuthorService.AuthorServiceClient _authorServiceClient;
    private readonly IMapper _mapper;

    public AuthorGrpcService(
        AuthorService.AuthorServiceClient authorServiceClient,
        IMapper mapper
    )
    {
        _authorServiceClient = authorServiceClient;
        _mapper = mapper;
    }

    public async Task<AuthorDto> GetAuthorAsync(long id)
    {
        var response = await _authorServiceClient.GetAuthorAsync(new AuthorGetRequest { AuthorId = id });
        return _mapper.Map<AuthorDto>(response.Author);
    }
}
