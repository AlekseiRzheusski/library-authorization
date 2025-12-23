namespace LibraryAuthorization.Application.Services.DTOs.BorrowingModels;

public class BorrowingListDto
{
    public IEnumerable<BorrowingDto> Borrowings { get; set; } = null!;
    public int TotalCount { get; set; }
    public int NumberOfPages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
