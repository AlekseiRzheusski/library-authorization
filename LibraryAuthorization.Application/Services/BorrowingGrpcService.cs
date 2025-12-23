using LibraryAuthorization.Application.Services.DTOs.BorrowingModels;
using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Enums;
using Librarymanagement;

using AutoMapper;

namespace LibraryAuthorization.Application.Services;

public class BorrowingGrpcsService : IBorrowingGrpcService
{
    private readonly IMapper _mapper;
    private readonly BorrowingService.BorrowingServiceClient _borrowingServiceClient;
    public BorrowingGrpcsService(
        BorrowingService.BorrowingServiceClient borrowingServiceClient,
        IMapper mapper
    )
    {
        _borrowingServiceClient = borrowingServiceClient;
        _mapper = mapper;
    }

    public async Task<BorrowingDto> BorrowBookAsync(BorrowBookCommand command, long userId)
    {
        if (command.daysToReturn is null)
            command.daysToReturn = 14;

        var request = new BorrowBookRequest{UserId = userId, BookId = command.BookId, DaysToReturn = command.daysToReturn.GetValueOrDefault()};
        var response = await _borrowingServiceClient.BorrowBookAsync(request);
        return _mapper.Map<BorrowingDto>(response);
    }

    public async Task<BorrowingListDto> GetUserBorrowingsAsync(
        long userId,
        BorrowingStatus? status,
        int pageNumber,
        int pageSize
    )
    {
        var request = new UserBorrowingsRequest
        {
            UserId = userId,
            Status = status.ToString(),
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var response = await _borrowingServiceClient.GetUserBorrowingsAsync(request);
        return _mapper.Map<BorrowingListDto>(response);
    }
}
