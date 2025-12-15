using LibraryAuthorization.Domain.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryAuthorization.Infrastructure.Data;

public class AuthDbContext : IdentityDbContext<LibraryUser, LibraryRole, long>
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LibraryRole>().HasData(
            new LibraryRole
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new LibraryRole
            {
                Id = 2,
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}
