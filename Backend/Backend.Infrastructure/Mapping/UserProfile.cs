using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Application.DTOs;

namespace Backend.Infrastructure.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        
        // Entity -> Domain Mapping
        CreateMap<UserEntity, User>(MemberList.None)
            .ConstructUsing((src, ctx) =>
            {
                // map your VOs and scalars:
                var countryCodeResult = CountryCode.Create(src.Country);
                if (countryCodeResult.IsFailed)
                    throw new AutoMapperMappingException($"Bad country code: {src.Country}");
                
                var locationResult = Location.Create(src.City, countryCodeResult.Value);
                if (locationResult.IsFailed)
                    throw new AutoMapperMappingException($"Bad location: {src.City}, {src.Country}");
                
                var reputationResult = Reputation.Create(src.Reputation);
                if (reputationResult.IsFailed)
                    throw new AutoMapperMappingException($"Bad reputation: {src.Reputation}");
                
                Photo? photo = null;
                if (!string.IsNullOrEmpty(src.ProfilePicture))
                {
                    var photoResult = Photo.Create(src.ProfilePicture);
                    if (photoResult.IsFailed)
                        throw new AutoMapperMappingException("Bad profile picture URL");
                    photo = photoResult.Value;
                }

                var bioResult = BioString.Create(src.Bio ?? "");
                if (bioResult.IsFailed)
                    throw new AutoMapperMappingException($"Bad bio: {src.Bio}");

                var userBooks = Enumerable.Empty<UserBook>();
                if(src.UserBooks != null && src.UserBooks.Count != 0)
                {
                    userBooks = ctx.Mapper
                        .Map<IEnumerable<UserBook>>(src.UserBooks);
                }

                // map SocialMediaLinks if EF actually loaded them:
                var domainLinks = Enumerable.Empty<SocialMediaLink>();
                if (src.SocialMediaLinks != null && src.SocialMediaLinks.Count != 0)
                {
                    domainLinks = ctx.Mapper
                        .Map<IEnumerable<SocialMediaLink>>(src.SocialMediaLinks);
                }

                // reconstitute the full User, passing in your child entities:
                return User.Reconstitute(
                    id:           src.Id,
                    email:        src.Email!,
                    username:     src.UserName!,
                    firstName:    src.FirstName,
                    lastName:     src.LastName,
                    birthDate:    src.BirthDate,
                    location:     locationResult.Value,
                    reputation:   reputationResult.Value,
                    profilePicture: photo,
                    bio:            bioResult.Value,
                    wishlist:       src.Wishlist?.Select(x => x.GeneralBookId) ?? Enumerable.Empty<Guid>(),
                    following:      src.Following?.Select(x => x.FollowedId)   ?? [],
                    followers:      src.Followers?.Select(x => x.FollowerId)   ?? [],
                    blockedUsers:   src.BlockedUsers?.Select(x => x.BlockedId) ?? [],
                    ownedBooks:     userBooks,
                    socialMediaLinks: domainLinks
                );
            });

        // Query Projection for database operations
        CreateMap<UserEntity, UserProjection>()
            .ForMember(dest => dest.LocationCity, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.LocationCountry, opt => opt.MapFrom(src => src.Country))
            .ReverseMap();
        
        // Domain -> Entity Mapping
        CreateMap<User, UserEntity>(MemberList.Source)
            // map your core scalar values:
            .ForMember(dest => dest.Email,            opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName,         opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.FirstName,        opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName,         opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.BirthDate,        opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.City,             opt => opt.MapFrom(src => src.Location.City))
            .ForMember(dest => dest.Country,          opt => opt.MapFrom(src => src.Location.Country.Code))
            .ForMember(dest => dest.ProfilePicture,   opt => opt.MapFrom(src => src.ProfilePicture))
            .ForMember(dest => dest.Bio,              opt => opt.MapFrom(src => src.Bio.Value))
            .ForMember(dest => dest.Reputation,       opt => opt.MapFrom(src => src.Reputation.Value))
            
            // explicitly IGNORE your relations so they stay as they are loaded in 'existing'—
            .ForMember(dest => dest.Wishlist,         opt => opt.Ignore())
            .ForMember(dest => dest.Following,        opt => opt.Ignore())
            .ForMember(dest => dest.Followers,        opt => opt.Ignore())
            .ForMember(dest => dest.BlockedUsers,     opt => opt.Ignore())
            .ForMember(dest => dest.UserBooks,        opt => opt.Ignore())
            .ForMember(dest => dest.SocialMediaLinks, opt => opt.Ignore())
            .AfterMap((src, dest) => 
            {
                // EF Core will handle collection updates via change tracking
                // Don't overwrite collections, just map scalar values
            });
        
    }

}
