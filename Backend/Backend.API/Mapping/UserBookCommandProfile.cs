using AutoMapper;
using Backend.API.DTOs.UserBooks;
using Backend.API.DTOs.UserBooks.Responses;
using Backend.API.DTOs.Common;


using Backend.Application.Commands.UserBooks.Bookmarks;
using Backend.Application.Commands.UserBooks.Core;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Application.ReadModels.UserBooks;



namespace Backend.API.Mapping;

public class UserBookCommandProfile : Profile
{
    public UserBookCommandProfile()
    {
        // UserBook Commands
        CreateMap<CreateUserBookRequest, CreateUserBookCommand>();
        CreateMap<UpdateUserBookRequest, UpdateUserBookCommand>();
        CreateMap<UpdateCoverRequest, UpdateUserBookCoverCommand>();
        CreateMap<ConfirmCoverRequest, ConfirmUBCoverCommand>();

        // Bookmark Commands
        CreateMap<CreateBookmarkRequest, CreateBookmarkCommand>();
        CreateMap<UpdateBookmarkRequest, UpdateBookmarkCommand>();

        // Responses
        CreateMap<(Guid, string), CreateUserBookResponse>()
            .ForMember(dest => dest.UserBookId, opt => opt.MapFrom(src => src.Item1))
            .ForMember(dest => dest.ImageKey, opt => opt.MapFrom(src => src.Item2));

        CreateMap<UserBook, UserBookResponse>(); // insted we should return the read model object
        CreateMap<Bookmark, BookmarkResponse>(); // insted we should return the read model object

        // Read models → DTOs
        CreateMap<UserBookDetailsReadModel, UserBookDetailsResponse>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        
        CreateMap<UserOwnBookProfileReadModel, UserOwnBookProfileResponse>();
        CreateMap<SwapUserBookListItem, SwapUserBookListItemResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        
        CreateMap<UserLibraryListItem, UserLibraryItemResponse>();
        CreateMap<UserBookListItem, UserBookListItemResponse>();
        CreateMap<BookmarkReadModel, BookmarkResponse>();

        // Pagination
        CreateMap<PaginatedResult<UserLibraryListItem>, PaginatedResponse<UserLibraryItemResponse>>();
        CreateMap<PaginatedResult<UserBookListItem>, PaginatedResponse<UserBookListItemResponse>>();
        CreateMap<PaginatedResult<BookmarkReadModel>, PaginatedResponse<BookmarkResponse>>();
    }
}