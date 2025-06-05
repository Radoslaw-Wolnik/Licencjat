// UserBookReadModelProfile.cs
using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.UserBooks;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Mapping;

public class UserBookReadModelProfile : Profile
{
    public UserBookReadModelProfile()
    {
        // UserBookProjection (for ListAsync)
        CreateMap<UserBookEntity, UserBookProjection>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("UserId", opt => opt.MapFrom(src => src.User.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Book.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Book.Author))
            .ForCtorParam("UserName", opt => opt.MapFrom(src => src.User.UserName))
            .ForCtorParam("UserReputation", opt => opt.MapFrom(src => src.User.Reputation))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.User.ProfilePicture))
            .ForCtorParam("CoverPhoto", opt => opt.MapFrom(src => src.CoverPhoto));

        // mby needed the other way oround - from projection to the user book entity
        /*
        CreateMap<UserBookProjection, UserBookEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.User.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Book.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Book.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.User.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.User.Reputation, opt => opt.MapFrom(src => src.UserReputation))
            .ForAllMembers(opt => opt.Ignore());
        */

        CreateMap<UserBookEntity, UserBookListItem>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Book.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Book.Author))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverPhoto))
            .ForCtorParam("User", opt => opt.MapFrom(src => new UserSmallReadModel(
                src.UserId,
                src.User.UserName ?? "__no__username__error__",
                src.User.ProfilePicture, // Im not sure if needed in the projection
                src.User.Reputation,
                null,
                null,
                null
            )))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForAllMembers(opt => opt.Ignore());

        // UserLibraryListItem
        CreateMap<UserBookEntity, UserLibraryListItem>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Book.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Book.Author))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverPhoto))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("RatingAvg", opt => opt.MapFrom(src =>
                src.Book.Reviews.Any() ? src.Book.Reviews.Average(r => r.Rating) : 7f));

        // UserBookDetailsReadModel
        CreateMap<UserBookEntity, UserBookDetailsReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Book.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Book.Author))
            .ForCtorParam("LanguageCode", opt => opt.MapFrom(src => src.Language))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src => src.PageCount))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src => src.CoverPhoto))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForAllMembers(opt => opt.Ignore());

        // BookmarkReadModel
        CreateMap<BookmarkEntity, BookmarkReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Page", opt => opt.MapFrom(src => src.Page))
            .ForCtorParam("Colour", opt => opt.MapFrom(src => src.Colour))
            .ForCtorParam("Description", opt => opt.MapFrom(src => src.Description))
            .ForAllMembers(opt => opt.Ignore());

        // UserOwnBookProfileReadModel (complex mapping)
        CreateMap<UserBookEntity, UserOwnBookProfileReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Book.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Book.Author))
            .ForCtorParam("LanguageCode", opt => opt.MapFrom(src => src.Language))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src => src.PageCount))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src => src.CoverPhoto))
            .ForCtorParam("State", opt => opt.MapFrom(src => src.State))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("UserReview", opt => opt.MapFrom(src =>
                src.Book.Reviews.FirstOrDefault(r => r.UserId == src.UserId)))
            .ForCtorParam("Swaps", opt => opt.MapFrom(src => src.SubSwaps.Select(s => s.Swap)))
            .ForCtorParam("Bookmarks", opt => opt.MapFrom(src =>
                src.Bookmarks.OrderByDescending(b => b.Page).Take(10)));

        // SwapEntity to SwapBookListItem
        CreateMap<SwapEntity, SwapUserBookListItem>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Username", opt => opt.MapFrom((src, ctx) =>
                ctx.Items["CurrentUserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapAccepting.User.UserName
                    : src.SubSwapRequesting.User.UserName))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom((src, ctx) =>
                ctx.Items["CurrentUserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapRequesting.UserBookReading!.CoverPhoto
                    : src.SubSwapAccepting.UserBookReading?.CoverPhoto))
            .ForAllMembers(opt => opt.Ignore());
    }
}