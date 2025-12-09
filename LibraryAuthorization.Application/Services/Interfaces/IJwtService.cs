using LibraryAuthorization.Domain.Entities;

namespace LibraryAuthorization.Application.Services.Interfaces;

public interface IJwtService
{
    public Task<string> GenerateJwtToken(LibraryUser user);
}
