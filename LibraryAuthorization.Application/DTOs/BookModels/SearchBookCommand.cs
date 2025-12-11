namespace LibraryAuthorization.Application.Services.DTOs.BookModels;

public class SearchBookCommand
{
    public string? Title { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public long? AuthorId { get; set; }
    public long? CategoryId { get; set; }
    public string? PublishedDate { get; set; }
    public int? PageCount { get; set; }
    public bool? IsAvailable { get; set; }

}
