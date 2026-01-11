using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LibraryAuthorization.Integration.Tests.Fixtures;

public class TestAuthContext
{
    public List<Claim> Claims { get; } = new();
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestAuthContext _context;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TestAuthContext context)
        : base(options, logger, encoder)
    {
        _context = context;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = _context.Claims.Any()
            ? _context.Claims
            : new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
            };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
