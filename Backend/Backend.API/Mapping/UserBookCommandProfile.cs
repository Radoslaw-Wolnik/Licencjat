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
        CreateMap<CreateUserBookRequest, CreateUserBookCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId)) // from controller
            .ForCtorParam("BookId", opt => opt.MapFrom(src => src.BookId))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForCtorParam("Language", opt => opt.MapFrom(src => src.Language))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src => src.PageCount))
            .ForCtorParam("CoverFileName", opt => opt.MapFrom(src => src.CoverFileName));

        CreateMap<UpdateUserBookRequest, UpdateUserBookCommand>()
            // .ForCtorParam("UserBookId", opt => opt.MapFrom(src => src.UserBookId)) // from controller
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State));

        CreateMap<UpdateCoverRequest, UpdateUserBookCoverCommand>()
            // .ForCtorParam("UserBookId", opt => opt.MapFrom(src => src.UserBookId)) // from controller
            .ForCtorParam("CoverFileName", opt => opt.MapFrom(src => src.CoverFileName));

        CreateMap<ConfirmCoverRequest, ConfirmUBCoverCommand>()
            // .ForCtorParam("UserBookId", opt => opt.MapFrom(src => src.UserBookId)) // from controller
            .ForCtorParam("ImageObjectKey", opt => opt.MapFrom(src => src.ImageObjectKey));

        // Bookmark Commands
        CreateMap<CreateBookmarkRequest, CreateBookmarkCommand>()
            // .ForCtorParam("UserBookId", opt => opt.MapFrom(src => src.UserBookId)) // from controller
            .ForCtorParam("Colour", opt => opt.MapFrom(src => src.Colour))
            .ForCtorParam("Page", opt => opt.MapFrom(src => src.Page))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description));

        CreateMap<UpdateBookmarkRequest, UpdateBookmarkCommand>()
            // .ForCtorParam("BookmarkId", opt => opt.MapFrom(src => src.BookmarkId)) // from controller
            .ForCtorParam("Colour", opt => opt.MapFrom(src => src.Colour))
            .ForCtorParam("Page", opt => opt.MapFrom(src => src.Page))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description));

        // Responses
        CreateMap<(Guid, string), CreateUserBookResponse>()
            .ForCtorParam("UserBookId", opt => opt.MapFrom(src => src.Item1))
            .ForCtorParam("ImageKey", opt => opt.MapFrom(src => src.Item2));

        CreateMap<UserBook, UserBookResponse>() // insted we should return the read model object
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("UserId", opt => opt.MapFrom(src => src.OwnerId))
            .ForCtorParam("BookId", opt => opt.MapFrom(src => src.GeneralBookId))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForCtorParam("Language", opt => opt.MapFrom(src => src.Language))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src => src.PageCount))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverPhoto))
        ; // for all other ignore ;

        CreateMap<Bookmark, BookmarkResponse>() // insted we should return the read model object
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Colour", opt => opt.MapFrom(src => src.Colour))
            .ForCtorParam("Page", opt => opt.MapFrom(src => src.Page))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description))
            ; // for all others ignore ;

        // Read models → DTOs
        CreateMap<UserBookDetailsReadModel, UserBookDetailsResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("LanguageCode", opt => opt.MapFrom(src => src.LanguageCode))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src => src.PageCount))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src => src.CoverPhotoUrl))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State.ToString()))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<UserOwnBookProfileReadModel, UserOwnBookProfileResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("LanguageCode", opt => opt.MapFrom(src => src.LanguageCode))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src => src.PageCount))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src => src.CoverPhotoUrl))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("UserReview", opt => opt.MapFrom(src => src.UserReview))
            .ForCtorParam("Swaps", opt => opt.MapFrom(src => src.Swaps.ToList()))
            .ForCtorParam("Bookmarks", opt => opt.MapFrom(src => src.Bookmarks.ToList()));

        CreateMap<SwapUserBookListItem, SwapUserBookListItemResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Username", opt => opt.MapFrom(src => src.Username))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src => src.CoverPhotoUrl));

        CreateMap<UserLibraryListItem, UserLibraryItemResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverUrl))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("RatingAvg", opt => opt.MapFrom(src => src.RatingAvg));

        CreateMap<UserBookListItem, UserBookListItemResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverUrl))
            .ForCtorParam("User", opt => opt.MapFrom(src => src.User))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State));

        CreateMap<BookmarkReadModel, BookmarkResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            // .ForCtorParam("UserBookId", opt => opt.MapFrom(src => src.UserBookId)) // apparently i felt like this is not needed here
            .ForCtorParam("Colour", opt => opt.MapFrom(src => src.Colour))
            .ForCtorParam("Page", opt => opt.MapFrom(src => src.Page))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description));

        // Pagination
        CreateMap<PaginatedResult<UserLibraryListItem>, PaginatedResponse<UserLibraryItemResponse>>();
        CreateMap<PaginatedResult<UserBookListItem>, PaginatedResponse<UserBookListItemResponse>>();
        CreateMap<PaginatedResult<BookmarkReadModel>, PaginatedResponse<BookmarkResponse>>();
    }
}