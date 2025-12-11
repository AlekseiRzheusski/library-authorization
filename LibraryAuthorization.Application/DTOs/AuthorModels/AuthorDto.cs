namespace LibraryAuthorization.Application.Services.DTOs.AuthorModels;

public class AuthorDto
{
    public long AuthorId {get; set;}
    public string FirstName {get; set;} = null!;
    public string LastName {get; set;} = null!;
    public string? Biography {get; set;}
    public string? DateOfBirth {get; set;}
    public bool IsActive {get; set;}
    public int BookCount {get; set;}
}
