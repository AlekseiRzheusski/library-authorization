namespace LibraryAuthorization.Application.DTOs.AuthModels;

public class LoginUserCommand
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
