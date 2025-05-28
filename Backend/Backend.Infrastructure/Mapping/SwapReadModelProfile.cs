using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Swaps;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Mapping;

public class SwapReadModelProfile : Profile
{
    public SwapReadModelProfile()
    {
        // SwapListItem with full user mapping
        CreateMap<SwapEntity, SwapListItem>()
            .ForMember(dest => dest.MyBookCoverUrl, opt => opt.MapFrom((src, _, _, ctx) =>
                ctx.Items["UserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapRequesting.UserBookReading!.CoverPhoto
                    : src.SubSwapAccepting.UserBookReading?.CoverPhoto))
            .ForMember(dest => dest.TheirBookCoverUrl, opt => opt.MapFrom((src, _, _, ctx) =>
                !ctx.Items["UserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapRequesting.UserBookReading!.CoverPhoto
                    : src.SubSwapAccepting.UserBookReading?.CoverPhoto))
            .ForMember(dest => dest.User, opt => opt.MapFrom((src, _, _, ctx) =>
                ctx.Items["UserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapAccepting.User // Map entire user entity
                    : src.SubSwapRequesting.User))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        // TimelineUpdate with user details
        CreateMap<TimelineEntity, TimelineUpdateReadModel>()
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.User.ProfilePicture));

        // Main Swap → Details mapping
        CreateMap<SwapEntity, SwapDetailsReadModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MySubSwap, opt => opt.MapFrom(src => src.SubSwapRequesting))
            .ForMember(dest => dest.TheirSubSwap, opt => opt.MapFrom(src => src.SubSwapAccepting))
            .ForMember(dest => dest.SocialMediaLinks, opt => opt.MapFrom((src, _, _, ctx) =>
                ctx.Items.ContainsKey("CurrentUserId") && (Guid)ctx.Items["CurrentUserId"] == src.SubSwapRequesting.UserId
                    ? src.SubSwapAccepting.User.SocialMediaLinks
                    : src.SubSwapRequesting.User.SocialMediaLinks))
            .ForMember(dest => dest.Updates, opt => opt.MapFrom((src, _, _, ctx) =>
                src.TimelineUpdates
                    .OrderByDescending(t => t.CreatedAt)
                    .Take((int)ctx.Items["MaxUpdates"])))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastStatus, opt => opt.MapFrom(src => src.Status));
            
        // SubSwapEntity → SubSwapReadModel
        CreateMap<SubSwapEntity, SubSwapReadModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                src.UserBookReading!.Book.Title))
            .ForMember(dest => dest.CoverPhotoUrl, opt => opt.MapFrom(src =>
                src.UserBookReading!.CoverPhoto))
            .ForMember(dest => dest.PageCount, opt => opt.MapFrom(src =>
                src.UserBookReading!.PageCount))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                src.User.UserName ?? "__no__username__error__"))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src =>
                src.User.ProfilePicture));
        
    }
}
