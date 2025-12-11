using AutoMapper;
using LibraryAuthorization.Application.Services.DTOs.AuthorModels;
using Librarymanagement;

namespace LibraryAuthorization.Application.Mappers;

public class AuthorMappingProfile: Profile
{
    public AuthorMappingProfile()
    {
        CreateMap<AuthorResponse, AuthorDto>();
    }
}
