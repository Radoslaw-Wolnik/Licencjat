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
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("MyBookCoverUrl", opt => opt.MapFrom((src, ctx) =>
                ctx.Items["UserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapRequesting.UserBookReading!.CoverPhoto
                    : src.SubSwapAccepting.UserBookReading?.CoverPhoto))
            .ForCtorParam("TheirBookCoverUrl", opt => opt.MapFrom((src, ctx) =>
                !ctx.Items["UserId"].Equals(src.SubSwapRequesting.UserId)
                    ? src.SubSwapRequesting.UserBookReading!.CoverPhoto
                    : src.SubSwapAccepting.UserBookReading?.CoverPhoto))
            .ForCtorParam(
        "User",
        opt => opt.MapFrom((src, ctx) =>
        {
            // Decide which UserEntity to pick:
            var chosenUserEntity = ctx.Items["UserId"].Equals(src.SubSwapRequesting.UserId)
                ? src.SubSwapAccepting.User
                : src.SubSwapRequesting.User;

            // Now explicitly map that UserEntity → UserSmallReadModel:
            // (requires that you have already registered a CreateMap<UserEntity, UserSmallReadModel>() somewhere)
            return ctx.Mapper.Map<UserSmallReadModel>(chosenUserEntity);
        }))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt));

        // TimelineUpdate with user details
        CreateMap<TimelineEntity, TimelineUpdateReadModel>()
            .ForCtorParam("Comment", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
            .ForCtorParam("UserName", opt => opt.MapFrom(src => src.User.UserName))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.User.ProfilePicture));

        // Main Swap → Details mapping
        CreateMap<SwapEntity, SwapDetailsReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("MySubSwap", opt => opt.MapFrom(src => src.SubSwapRequesting))
            .ForCtorParam("TheirSubSwap", opt => opt.MapFrom(src => src.SubSwapAccepting))
            .ForCtorParam(
                  "SocialMediaLinks",
                  opt => opt.MapFrom((src, ctx) =>
                  {
                      var currentUserId = (Guid)ctx.Items["CurrentUserId"];
                      bool isRequestingUser = currentUserId == src.SubSwapRequesting.UserId;

                      // Pick the correct UserEntity
                      var chosenUser = isRequestingUser
                          ? src.SubSwapAccepting?.User
                          : src.SubSwapRequesting?.User;

                      // If chosenUser is null or has no SocialMediaLinks, return empty
                      return chosenUser?.SocialMediaLinks 
                             ?? Enumerable.Empty<SocialMediaLinkEntity>();
                  })
                )
            .ForCtorParam("LastStatus", opt => opt.MapFrom(src => src.Status))
            .ForCtorParam("Updates", opt => opt.MapFrom((src, ctx) =>
                src.TimelineUpdates
                    .OrderByDescending(t => t.CreatedAt)
                    .Take((int)ctx.Items["MaxUpdates"])))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt));
            
            
        // SubSwapEntity → SubSwapReadModel
        CreateMap<SubSwapEntity, SubSwapReadModel>()
            .ForCtorParam("Title", opt => opt.MapFrom(src =>
                src.UserBookReading!.Book.Title))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src =>
                src.UserBookReading!.CoverPhoto))
            .ForCtorParam("PageCount", opt => opt.MapFrom(src =>
                src.UserBookReading!.PageCount))
            .ForCtorParam("UserName", opt => opt.MapFrom(src =>
                src.User.UserName ?? "__no__username__error__"))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src =>
                src.User.ProfilePicture));
        
    }
}
