using LibraryAuthorization.Application.DTOs.AuthModels;
using LibraryAuthorization.Domain.Entities;

namespace LibraryAuthorization.Application.Services.Interfaces;

public interface IJwtService
{
    public Task<TokensDto> GenerateJwtToken(LibraryUser user);
}
