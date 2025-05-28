using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Mapping;

public class UserReadModelProfile : Profile
{
    public UserReadModelProfile()
    {
        CreateMap<UserEntity, UserSmallReadModel>()
            // Always mapped
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePicture))

            // Conditionally mapped
            // .ForMember(dest => dest.UserReputation, opt => opt.MapFrom<ReputationResolver>())
            .ForMember(dest => dest.UserReputation, opt => opt.MapFrom((src, _, _, ctx) => 
                ctx.Items.ContainsKey("IncludeDetails") ? src.Reputation : (float?)null))
            .ForMember(dest => dest.City, opt => opt.MapFrom((src, dest, _, ctx) =>
                ctx.Items.ContainsKey("IncludeDetails") ? src.City : null))
            .ForMember(dest => dest.Country, opt => opt.MapFrom((src, dest, _, ctx) =>
                ctx.Items.ContainsKey("IncludeDetails") ? src.Country : null))
            .ForMember(dest => dest.SwapCount, opt => opt.MapFrom((src, _, _, ctx) => 
                ctx.Items.ContainsKey("IncludeDetails") 
                    ? src.SubSwaps.Count(ss => 
                        ss.Swap.Status != SwapStatus.Requested && 
                        ss.Swap.Status != SwapStatus.Disputed)
                    : (int?)null));


        // UserProfileReadModel mapping
        CreateMap<UserEntity, UserProfileReadModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Reputation, opt => opt.MapFrom(src => src.Reputation))
            .ForMember(dest => dest.SwapCount, opt => opt.MapFrom(src =>
                src.SubSwaps.Count(ss =>
                    ss.Swap.Status != SwapStatus.Requested &&
                    ss.Swap.Status != SwapStatus.Disputed)))
            .ForMember(dest => dest.SocialMsdias, opt => opt.MapFrom(src =>
                src.SocialMediaLinks))
            .ForMember(dest => dest.Wishlist, opt => opt.MapFrom(src =>
                src.Wishlist.Select(w => w.GeneralBook)))
            .ForMember(dest => dest.Reading, opt => opt.MapFrom(src =>
                src.UserBooks.Where(ub => ub.Status == BookStatus.Reading)))
            .ForMember(dest => dest.UserLibrary, opt => opt.MapFrom(src =>
                src.UserBooks.Where(ub => ub.Status != BookStatus.Reading)));

        // SocialMediaLink → SocialMediaLinkReadModel
        CreateMap<SocialMediaLinkEntity, SocialMediaLinkReadModel>()
            .ForMember(dest => dest.Platform, opt => opt.MapFrom(src => src.Platform))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));

    }
    /*
    private class ReputationResolver : IValueResolver<UserEntity, UserSmallReadModel, float?>
    {
        public float? Resolve(UserEntity source, UserSmallReadModel destination, float? destMember, ResolutionContext context)
        {
            return context.Items.ContainsKey("IncludeDetails") ? source.Reputation : null;
        }
    }
    */
}