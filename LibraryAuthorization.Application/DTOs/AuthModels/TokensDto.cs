namespace LibraryAuthorization.Application.DTOs.AuthModels;

public class TokensDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
