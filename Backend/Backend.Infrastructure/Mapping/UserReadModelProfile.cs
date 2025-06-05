using System.Reflection.Emit;
using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Microsoft.CodeAnalysis;

namespace Backend.Infrastructure.Mapping;

public class UserReadModelProfile : Profile
{
    public UserReadModelProfile()
    {

        // CreateMap<UserEntity, UserSmallReadModel>(MemberList.None)
        //     .ConvertUsing<UserSmallReadModelConverter>();
        CreateMap<UserEntity, UserSmallReadModel>(MemberList.None)
            .ConstructUsing((src, ctx) =>
            {
                bool includeDetails = false;

                if (ctx.TryGetItems(out var items) &&
                    items?.TryGetValue("IncludeDetails", out var raw) == true)
                {
                    includeDetails = raw is bool b && b;
                }

                return new UserSmallReadModel(
                    UserId: src.Id,
                    Username: src.UserName ?? "__missing_username_error__",
                    ProfilePictureUrl: src.ProfilePicture,
                    UserReputation: includeDetails ? (float?)src.Reputation : null,
                    City: includeDetails ? src.City : null,
                    Country: includeDetails ? src.Country : null,
                    SwapCount: includeDetails
                        ? src.SubSwaps?
                            .Where(ss => ss?.Swap != null)
                            .Count(ss => ss!.Swap.Status != SwapStatus.Requested &&
                                        ss.Swap.Status != SwapStatus.Disputed)
                        : null
                );
            })
            .ForAllMembers(opt => opt.Ignore());
        // .AfterMap((src, dest) => {});
        // .ForAllMembers(opt => opt.Ignore())
        // .ForMember(dest => dest.UserReputation, opt => opt.Ignore())
        // .ForMember(dest => dest.SwapCount, opt => opt.Ignore());

        // SocialMediaLink → SocialMediaLinkReadModel
        CreateMap<SocialMediaLinkEntity, SocialMediaLinkReadModel>()
            .ForCtorParam("Platform", opt => opt.MapFrom(src => src.Platform))
            .ForCtorParam("Url", opt => opt.MapFrom(src => src.Url));

        // UserProfileReadModel mapping
        CreateMap<UserEntity, UserProfileReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("UserName", opt => opt.MapFrom(src => src.UserName))
            .ForCtorParam("Reputation", opt => opt.MapFrom(src => src.Reputation))
            .ForCtorParam("SwapCount", opt => opt.MapFrom(src =>
                src.SubSwaps == null
                ? (int?)null
                : (int?)src.SubSwaps
                    .Where(ss => ss.Swap != null)
                    .Count(ss =>
                        ss!.Swap.Status != SwapStatus.Requested &&
                        ss.Swap.Status != SwapStatus.Disputed
                    )))
            .ForCtorParam("City", opt => opt.MapFrom(src => src.City))
            .ForCtorParam("Country", opt => opt.MapFrom(src => src.Country))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.ProfilePicture))
            .ForCtorParam("Bio", opt => opt.MapFrom(src => src.Bio))
            .ForCtorParam("SocialMedias", opt => opt.MapFrom(src =>
                src.SocialMediaLinks))
            .ForCtorParam(
      "Wishlist",
      opt => opt.MapFrom(src => src.Wishlist
              .Where(w => w.GeneralBook != null)
              .Select(w => w.GeneralBook!) // 
              .ToList()
      )
    )

    // 2) Reading: only the “Book” of each UserBook where status == Reading
    .ForCtorParam(
      "Reading",
      opt => opt.MapFrom(src => src.UserBooks
              .Where(ub => ub.Status == BookStatus.Reading && ub.Book != null)
              .Select(ub => ub.Book!)
              .ToList()
      )
    )

    // 3) UserLibrary: all UserBooks’ Book, regardless of status
    .ForCtorParam(
      "UserLibrary",
      opt => opt.MapFrom(src => src.UserBooks
              .Where(ub => ub.Book != null)
              .Select(ub => ub.Book!)
              .ToList()
      )
    )
            .ForAllMembers(opt => opt.Ignore());




        // move to the book read models profiles later on ----------------------
        // UserBookEntity -> BookCoverItemReadModel
        CreateMap<UserBookEntity, BookCoverItemReadModel>()
                .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("Title", opt => opt.MapFrom(src => src.Book.Title)) // from general book
                .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverPhoto))
                .ForAllMembers(opt => opt.Ignore());
    
        // GeneralBook -> BookCoverItemReadModel
        CreateMap<GeneralBookEntity, BookCoverItemReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverPhoto))
            .ForAllMembers(opt => opt.Ignore());
        
    }


    
            

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

// CreateMap<UserEntity, UserSmallReadModel>(MemberList.None)
        //     .ConvertUsing<UserSmallReadModelConverter>();
        /*
        CreateMap<UserEntity, UserSmallReadModel>(MemberList.None)
            .ConstructUsing((src, ctx) =>
            {
                bool includeDetails = ctx.Items.ContainsKey("IncludeDetails");

                return new UserSmallReadModel(
                    UserId: src.Id,
                    Username: src.UserName ?? "__missing_username_error__",
                    ProfilePictureUrl: src.ProfilePicture,
                    UserReputation: includeDetails ? (float?)src.Reputation : null,
                    City: includeDetails ? src.City : null,
                    Country: includeDetails ? src.Country : null,
                    SwapCount: includeDetails
                        ? src.SubSwaps?
                            .Where(ss => ss?.Swap != null)
                            .Count(ss => ss!.Swap.Status != SwapStatus.Requested &&
                                        ss.Swap.Status != SwapStatus.Disputed)
                        : null
                );
            })
            .ForAllMembers(opt => opt.Ignore());
        // .AfterMap((src, dest) => {});
        // .ForAllMembers(opt => opt.Ignore())
        // .ForMember(dest => dest.UserReputation, opt => opt.Ignore())
        // .ForMember(dest => dest.SwapCount, opt => opt.Ignore());
        */