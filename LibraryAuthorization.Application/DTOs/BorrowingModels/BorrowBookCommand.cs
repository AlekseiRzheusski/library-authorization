namespace LibraryAuthorization.Application.Services.DTOs.BorrowingModels;

public class BorrowBookCommand
{
    public long BookId { get; set; }
    public int? daysToReturn { get; set; }
}
