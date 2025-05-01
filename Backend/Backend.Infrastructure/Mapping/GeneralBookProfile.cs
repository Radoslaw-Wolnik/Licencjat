using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Application.DTOs;
using Backend.Domain.Enums;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class GeneralBookProfile : Profile
{
    public GeneralBookProfile()
    {
        // Entity -> Domain Mapping
        CreateMap<GeneralBookEntity, GeneralBook>(MemberList.None)
            .ConstructUsing(src => MapToDomain(src))
            .ForAllMembers(opt => opt.Ignore());

        // Query Projection for database operations
        CreateMap<GeneralBookEntity, BookProjection>()
            .ReverseMap();
        
        // Domain -> Entity Mapping
        CreateMap<GeneralBook, GeneralBookEntity>(MemberList.Source)
        // map core scalar values
        .ForMember(dest => dest.Id,            opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Title,         opt => opt.MapFrom(src => src.Title))
        .ForMember(dest => dest.Author,        opt => opt.MapFrom(src => src.Author))
        .ForMember(dest => dest.Published,     opt => opt.MapFrom(src => src.Published))
        .ForMember(dest => dest.Language,      opt => opt.MapFrom(src => src.OriginalLanguage.Code))
        .ForMember(dest => dest.CoverPhoto,    opt => opt.MapFrom(src => src.CoverPhoto))
        .ForMember(dest => dest.Genres,        opt => opt.MapFrom(src => src.Genres))

        // ignore the RatingAvg *source* property so MemberList.Source is happy
        .ForSourceMember(src => src.RatingAvg, opt => opt.DoNotValidate())

        // explicitly IGNORE relations
        .ForMember(dest => dest.UserBooks,         opt => opt.Ignore())
        .ForMember(dest => dest.Reviews,           opt => opt.Ignore())
        .ForMember(dest => dest.WishlistedByUsers, opt => opt.Ignore());
        
    }

    private GeneralBook MapToDomain(GeneralBookEntity src)
    {
        // Handle CountryCode creation
        var languageCodeResult = LanguageCode.Create(src.Language);
        if (languageCodeResult.IsFailed)
            throw new AutoMapperMappingException(
                $"Invalid country code: {string.Join(", ", languageCodeResult.Errors)}");
        
        // handle cover photo creation
        var photoResult = Photo.Create(src.CoverPhoto);
            if (photoResult.IsFailed)
                throw new AutoMapperMappingException(
                    $"Invalid reputation: {string.Join(", ", photoResult.Errors)}");
        
        // Handle calculating the total Review Score
        var avgScore = src.Reviews.Count != 0
            ? src.Reviews.Average(r => r.Rating)
            : 0.0f;
        
        
        var ratingAvg = Rating.Create((float)avgScore).Value;
        
        return GeneralBook.Reconstitute(
            src.Id,
            src.Title,
            src.Author,
            src.Published,
            languageCodeResult.Value,
            photoResult.Value,
            ratingAvg,
            src.Genres ?? Array.Empty<BookGenre>(),

            // if src.Wishlist is null (not loaded), use empty
            src.UserBooks?.Select(gb => gb.Id) ?? Enumerable.Empty<Guid>(),
            src.Reviews?.Select(gb => gb.Id)   ?? []
        );
    }

}
