using LibraryAuthorization.Application.Services.DTOs.AuthorModels;

namespace LibraryAuthorization.Application.Services.Interfaces;

public interface IAuthorGrpcService
{
    public Task<AuthorDto> GetAuthorAsync(long id);
}
