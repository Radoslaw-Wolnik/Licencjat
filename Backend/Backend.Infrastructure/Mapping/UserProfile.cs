using System.Linq;
using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Infrastructure.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Entity -> Domain via a single-expression ConstructUsing calling a static helper
            CreateMap<UserEntity, User>(MemberList.None)
                .ConstructUsing(src => CreateDomainUser(src))
                .ForAllMembers(opt => opt.Ignore());

            // Domain -> Entity (standard mappings)
            CreateMap<User, UserEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location.Country.Code))
                .ForMember(dest => dest.Reputation, opt => opt.MapFrom(src => src.Reputation.Value))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.Wishlist, opt => opt.Ignore())
                .ForMember(dest => dest.FollowedBooks, opt => opt.Ignore())
                .ForMember(dest => dest.Following, opt => opt.Ignore())
                .ForMember(dest => dest.Followers, opt => opt.Ignore())
                .ForMember(dest => dest.BlockedUsers, opt => opt.Ignore())
                .ForMember(dest => dest.UserBooks, opt => opt.Ignore())
                .ForMember(dest => dest.SocialMediaLinks, opt => opt.Ignore());
        }

        private static User CreateDomainUser(UserEntity src)
        {
            // Handle CountryCode creation
            var countryCodeResult = CountryCode.Create(src.Country);
            if (countryCodeResult.IsFailed)
                throw new AutoMapperMappingException(
                    $"Invalid country code: {string.Join(", ", countryCodeResult.Errors)}");

            // Handle Location creation
            var locationResult = Location.Create(src.City, countryCodeResult.Value);
            if (locationResult.IsFailed)
                throw new AutoMapperMappingException(
                    $"Invalid location: {string.Join(", ", locationResult.Errors)}");

            // Handle Reputation creation
            var reputationResult = Reputation.Create(src.Reputation);
            if (reputationResult.IsFailed)
                throw new AutoMapperMappingException(
                    $"Invalid reputation: {string.Join(", ", reputationResult.Errors)}");

            // Extract collection IDs
            var wishlist = src.Wishlist.Select(gb => gb.Id).ToList();
            var followedBooks = src.FollowedBooks.Select(gb => gb.Id).ToList();
            var following = src.Following.Select(f => f.FollowedId).ToList();
            var followers = src.Followers.Select(f => f.FollowerId).ToList();
            var blockedUsers = src.BlockedUsers.Select(b => b.BlockedId).ToList();
            var ownedBooks = src.UserBooks.Select(ub => ub.Id).ToList();
            var socialMediaLinks = src.SocialMediaLinks.Select(sml => sml.Id).ToList();

            // Reconstitute domain User
            return User.Reconstitute(
                src.Id,
                src.Email!,
                src.UserName!,
                src.FirstName,
                src.LastName,
                src.BirthDate,
                locationResult.Value,
                reputationResult.Value,
                src.ProfilePicture,
                src.Bio,
                wishlist,
                followedBooks,
                following,
                followers,
                blockedUsers,
                ownedBooks,
                socialMediaLinks
            );
        }
    }
}
