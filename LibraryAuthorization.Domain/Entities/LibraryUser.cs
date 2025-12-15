using Microsoft.AspNetCore.Identity;

namespace LibraryAuthorization.Domain.Entities;

public class LibraryUser : IdentityUser<long>
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
