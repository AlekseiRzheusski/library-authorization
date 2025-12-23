using LibraryAuthorization.Application.Services.DTOs.BorrowingModels;
using LibraryAuthorization.Domain.Enums;

namespace LibraryAuthorization.Application.Services.Interfaces;

public interface IBorrowingGrpcService
{
    public Task<BorrowingDto> BorrowBookAsync(BorrowBookCommand command, long userId);
    public Task<BorrowingListDto> GetUserBorrowingsAsync(
        long userId,
        BorrowingStatus? status,
        int pageNumber,
        int pageSize
    );
}
