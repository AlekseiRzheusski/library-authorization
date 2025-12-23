using AutoMapper;
using LibraryAuthorization.Application.Services.DTOs.BorrowingModels;
using Librarymanagement;

namespace LibraryAuthorization.Application.Mappers;

public class BorrowingMappingProfile: Profile
{
    public BorrowingMappingProfile()
    {
        CreateMap<BorrowingResponse, BorrowingDto>();
        CreateMap<BorrowingListResponse, BorrowingListDto>();

        CreateMap<BorrowBookCommand, BorrowBookRequest>();
    }
}
