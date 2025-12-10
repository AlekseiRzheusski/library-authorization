namespace LibraryAuthorization.Application.Services.DTOs.BookModels;

public class BookDto
{
    public long BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Isbn { get; set; } = null!;
    public string? Description { get; set; }
    public long AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? PublishedDate { get; set; }
    public int PageCount { get; set; }
    public bool IsAvailable { get; set; }
}
