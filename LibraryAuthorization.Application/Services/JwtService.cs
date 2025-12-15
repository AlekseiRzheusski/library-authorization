using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryAuthorization.Infrastructure.Repositories.Interfaces;
using LibraryAuthorization.Application.DTOs.AuthModels;

namespace LibraryAuthorization.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly UserManager<LibraryUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public JwtService(
        IConfiguration config,
        UserManager<LibraryUser> userManager,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _config = config;
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokensDto> GenerateJwtToken(LibraryUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        var refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveAsync();

        return new TokensDto{
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken.Token
        };
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
