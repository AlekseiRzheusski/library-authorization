namespace LibraryAuthorization.Application.Services.DTOs.BookModels;

public class BookListDto
{
    public IEnumerable<BookDto> Books { get; set; } = null!;
    public int TotalCount { get; set; }
    public int NumberOfPages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
