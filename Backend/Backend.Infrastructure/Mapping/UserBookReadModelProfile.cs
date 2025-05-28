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
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Book.Author))
            .ForMember(dest => dest.CoverPhoto, opt => opt.MapFrom(src => src.CoverPhoto))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.User.ProfilePicture))
            .ForMember(dest => dest.UserReputation, opt => opt.MapFrom(src => src.User.Reputation));

        // UserBookListItem
        CreateMap<UserBookProjection, UserBookListItem>()
            .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src => src.CoverPhoto))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserSmallReadModel(
                src.UserId,
                src.UserName,
                src.ProfilePictureUrl, // Im not sure if needed in the projection
                src.UserReputation,
                null,
                null,
                null
            )));

        // UserLibraryListItem
        CreateMap<UserBookEntity, UserLibraryListItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Book.Author))
            .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src => src.CoverPhoto))
            .ForMember(dest => dest.RatingAvg, opt => opt.MapFrom(src => 
                src.Book.Reviews.Any() ? src.Book.Reviews.Average(r => r.Rating) : 0f));

        // UserBookDetailsReadModel
        CreateMap<UserBookEntity, UserBookDetailsReadModel>()
            .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.CoverPhotoUrl, opt => opt.MapFrom(src => src.CoverPhoto));

        // BookmarkReadModel
        CreateMap<BookmarkEntity, BookmarkReadModel>();

        // UserOwnBookProfileReadModel (complex mapping)
        CreateMap<UserBookEntity, UserOwnBookProfileReadModel>()
            .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.CoverPhotoUrl, opt => opt.MapFrom(src => src.CoverPhoto))
            .ForMember(dest => dest.UserReview, opt => opt.MapFrom(src => 
                src.Book.Reviews.FirstOrDefault(r => r.UserId == src.UserId)))
            .ForMember(dest => dest.Swaps, opt => opt.MapFrom(src => src.SubSwaps.Select(s => s.Swap)))
            .ForMember(dest => dest.Bookmarks, opt => opt.MapFrom(src => 
                src.Bookmarks.OrderByDescending(b => b.Page).Take(10)));
        
        // SwapEntity to SwapBookListItem
        CreateMap<SwapEntity, SwapUserBookListItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom((src, dest, _, ctx) => 
                ctx.Items["CurrentUserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapAccepting.User.UserName
                    : src.SubSwapRequesting.User.UserName))
            .ForMember(dest => dest.CoverPhotoUrl, opt => opt.MapFrom((src, dest, _, ctx) => 
                ctx.Items["CurrentUserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapAccepting.UserBookReading?.CoverPhoto
                    : src.SubSwapRequesting.UserBookReading!.CoverPhoto))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
    }
}