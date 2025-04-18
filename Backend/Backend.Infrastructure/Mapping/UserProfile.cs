using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Application.DTOs;

namespace Backend.Infrastructure.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            
            // Entity -> Domain Mapping
            CreateMap<UserEntity, User>(MemberList.None)
                .ConstructUsing(src => MapToDomain(src))
                .ForAllMembers(opt => opt.Ignore());

            // Domain -> Entity Mapping
            CreateMap<User, UserEntity>()
                .ConvertUsing(src => MapToEntity(src));

            // Query Projection for database operations
            CreateMap<UserEntity, UserProjection>()
                .ForMember(dest => dest.LocationCity, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.LocationCountry, opt => opt.MapFrom(src => src.Country))
                .ReverseMap();
            
            /*
            CreateMap<User, UserProjection>()
            .ForMember(dest => dest.LocationCity, opt => opt.MapFrom(src => src.Location.City))
                .ForMember(dest => dest.LocationCountry, opt => opt.MapFrom(src => src.Location.Country))
                .ReverseMap();
            */

            /*
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
            
            // Entity -> Domain via a single-expression ConstructUsing calling a static helper
            CreateMap<UserEntity, User>(MemberList.None)
                .ConstructUsing(src => MapToDomain(src))
                .ForAllMembers(opt => opt.Ignore());
            
            */
            
        }

        private static User MapToDomain(UserEntity src)
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
                // if src.Wishlist is null (not loaded), use empty
                src.Wishlist?.Select(gb => gb.Id)               ?? Enumerable.Empty<Guid>(),
                src.FollowedBooks?.Select(gb => gb.Id)          ?? [],
                src.Following?.Select(f => f.FollowedId)        ?? [],
                src.Followers?.Select(f => f.FollowerId)        ?? [],
                src.BlockedUsers?.Select(b => b.BlockedId)      ?? [],
                src.UserBooks?.Select(ub => ub.Id)              ?? [],
                src.SocialMediaLinks?.Select(sml => sml.Id)     ?? []
            );
        }

        private static UserEntity MapToEntity(User user)
        {
            return new UserEntity
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                City = user.Location.City,
                Country = user.Location.Country.Code,
                Reputation = user.Reputation.Value,
                ProfilePicture = user.ProfilePicture,
                Bio = user.Bio,
                // Collections are handled through separate relationships
                Wishlist = new List<GeneralBookEntity>(),
                FollowedBooks = [],
                Following = [],
                Followers = [],
                BlockedUsers = [],
                UserBooks = [],
                SocialMediaLinks = []
            };
        }
    }

}