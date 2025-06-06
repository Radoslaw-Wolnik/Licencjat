using AutoMapper;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.Users;
using Backend.API.DTOs.Users.Responses;
using Backend.Application.Commands.Users.Blocked;
using Backend.Application.Commands.Users.Core;
using Backend.Application.Commands.Users.Following;
using Backend.Application.Commands.Users.ProfilePictures;
using Backend.Application.Commands.Users.SocialMedia;
using Backend.Application.Commands.Users.Wishlist;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.API.Mapping;

public class UserCommandProfile : Profile
{
    public UserCommandProfile()
    {
        // Core User
        CreateMap<UpdateProfileRequest, UpdateUserProfileCommand>()
            // .ForCtorParam("userId", opt => opt.MapFrom(src => src.Id)) its added by controller
            // idk ignore? 
            .ForCtorParam("City", opt => opt.MapFrom(src => src.City))
            .ForCtorParam("CountryCode", opt => opt.MapFrom(src => src.CountryCode))
            .ForCtorParam("Bio", opt => opt.MapFrom(src => src.Bio));

        // Blocked Users
        CreateMap<BlockUserRequest, AddBlockedUserCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.Id)) SAME AS ABOVE
            .ForCtorParam("UserBlockedId", opt => opt.MapFrom(src => src.UserBlockedId));

        // Following
        CreateMap<FollowUserRequest, AddFollowedUserCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.Id)) SAME AS ABOVE
            .ForCtorParam("UserFollowedId", opt => opt.MapFrom(src => src.UserFollowedId));

        // Social Media
        CreateMap<AddSocialMediaRequest, AddSocialMediaCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Platform", opt => opt.MapFrom(src => src.Platform))
            .ForCtorParam("Url", opt => opt.MapFrom(src => src.Url)); 

        CreateMap<UpdateSocialMediaRequest, UpdateSocialMediaCommand>()
            .ForCtorParam("SocialMediaId", opt => opt.MapFrom(src => src.SocialMediaId))
            .ForCtorParam("Platform", opt => opt.MapFrom(src => src.Platform))
            .ForCtorParam("Url", opt => opt.MapFrom(src => src.Url));

        // Wishlist
        CreateMap<WishlistBookRequest, AddWishlistBookCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.Id)) SAME AS ABOVE
            .ForCtorParam("WishlistBookId", opt => opt.MapFrom(src => src.BookId));

        CreateMap<WishlistBookRequest, RemoveWishlistBookCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.Id)) SAME AS ABOVE
            .ForCtorParam("WishlistBookId", opt => opt.MapFrom(src => src.BookId));

        // Profile Pictures
        CreateMap<UpdateProfilePictureRequest, UpdateUserProfilePictureCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("ProfilePictureFileName", opt => opt.MapFrom(src => src.ProfilePictureFileName));

        CreateMap<ConfirmProfilePictureRequest, ConfirmUserProfilePictureCommand>()
            // .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("ImageObjectKey", opt => opt.MapFrom(src => src.ImageObjectKey));

        // Responses
        CreateMap<User, UserProfileResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Username", opt => opt.MapFrom(src => src.Username))
            .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email))
            .ForCtorParam("City", opt => opt.MapFrom(src => src.Location.City))
            .ForCtorParam("CountryCode", opt => opt.MapFrom(src => src.Location.Country))
            .ForCtorParam("Bio", opt => opt.MapFrom(src => src.Bio))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.ProfilePicture))
            .ForAllMembers(opt => opt.Ignore());


        CreateMap<SocialMediaLinkReadModel, SocialMediaLinkResponse>()
            .ForCtorParam("Platform", opt => opt.MapFrom(src => src.Platform.ToString())) // or keep enum - but i would need to make it in frontend
            .ForCtorParam("Url", opt => opt.MapFrom(src => src.Url));

        // Read model → DTO
        CreateMap<UserSmallReadModel, UserSmallResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.UserId))
            .ForCtorParam("Username", opt => opt.MapFrom(src => src.Username))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.ProfilePictureUrl))
            .ForCtorParam("UserReputation", opt => opt.MapFrom(src => src.UserReputation))
            .ForCtorParam("City", opt => opt.MapFrom(src => src.City))
            .ForCtorParam("Country", opt => opt.MapFrom(src => src.Country))
            .ForCtorParam("SwapCount", opt => opt.MapFrom(src => src.SwapCount));

        CreateMap<UserProfileReadModel, UserProfileFullResponse>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Username", opt => opt.MapFrom(src => src.UserName))
            .ForCtorParam("Reputation", opt => opt.MapFrom(src => src.Reputation))
            .ForCtorParam("SwapCount", opt => opt.MapFrom(src => src.SwapCount))
            .ForCtorParam("City", opt => opt.MapFrom(src => src.City))
            .ForCtorParam("Country", opt => opt.MapFrom(src => src.Country))
            .ForCtorParam("ProfilePictureUrl", opt => opt.MapFrom(src => src.ProfilePictureUrl))
            .ForCtorParam("Bio", opt => opt.MapFrom(src => src.Bio))

            .ForCtorParam("SocialMedias", opt => opt.MapFrom(src => src.SocialMedias.ToList()))
            .ForCtorParam("Wishlist", opt => opt.MapFrom(src => src.Wishlist.ToList()))
            .ForCtorParam("Reading", opt => opt.MapFrom(src => src.Reading.ToList()))
            .ForCtorParam("UserLibrary", opt => opt.MapFrom(src => src.UserLibrary.ToList()));
    }
}