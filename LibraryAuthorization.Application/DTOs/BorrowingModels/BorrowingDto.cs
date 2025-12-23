namespace LibraryAuthorization.Application.Services.DTOs.BorrowingModels;

public class BorrowingDto
{
    public long BorrowingId { get; set; }
    public long BookId { get; set; }
    public string BookTitle { get; set; } = null!;
    public long UserId { get; set; }
    public string BorrowDate { get; set; } = null!;
    public string DueDate { get; set; } = null!;
    public string ReturnDate { get; set; } = null!;
    public string Status { get; set; } = null!;
    public double? FineAmount { get; set; }
}
