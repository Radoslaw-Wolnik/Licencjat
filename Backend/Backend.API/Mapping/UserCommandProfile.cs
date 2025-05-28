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
using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.API.Mapping;

public class UserCommandProfile : Profile
{
    public UserCommandProfile()
    {
        // Core User
        CreateMap<UpdateProfileRequest, UpdateUserProfileCommand>();

        // Blocked Users
        CreateMap<BlockUserRequest, AddBlockedUserCommand>();

        // Following
        CreateMap<FollowUserRequest, AddFollowedUserCommand>();

        // Social Media
        CreateMap<AddSocialMediaRequest, AddSocialMediaCommand>();
        CreateMap<UpdateSocialMediaRequest, UpdateSocialMediaCommand>();

        // Wishlist
        CreateMap<WishlistBookRequest, AddWishlistBookCommand>();
        CreateMap<WishlistBookRequest, RemoveWishlistBookCommand>();

        // Profile Pictures
        CreateMap<UpdateProfilePictureRequest, UpdateUserProfilePictureCommand>();
        CreateMap<ConfirmProfilePictureRequest, ConfirmUserProfilePictureCommand>();

        // Responses
        CreateMap<User, UserProfileResponse>();
        CreateMap<SocialMediaLink, SocialMediaResponse>();

        // Read model → DTO
        CreateMap<UserSmallReadModel, UserSmallResponse>();
    }
}