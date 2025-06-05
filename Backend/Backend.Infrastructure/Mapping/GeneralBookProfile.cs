using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Application.ReadModels.GeneralBooks;

namespace Backend.Infrastructure.Mapping;

public class GeneralBookProfile : Profile
{
    public GeneralBookProfile()
    {
        // Entity -> Domain Mapping    
        CreateMap<GeneralBookEntity, GeneralBook>(MemberList.None)
            .ConstructUsing((src, ctx) =>
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
                        $"Invalid photo: {string.Join(", ", photoResult.Errors)}");

                // Handle calculating the total Review Score
                var avgScore = src.Reviews.Count != 0
                    ? src.Reviews.Average(r => r.Rating)
                    : 7.0f; // mby nullable so thta we can write thta there is no avg yet


                var ratingAvg = Rating.Create((float)avgScore).Value;

                var userBooks = Enumerable.Empty<UserBook>();
                if (src.UserBooks != null && src.UserBooks.Count != 0)
                {
                    userBooks = ctx.Mapper
                        .Map<IEnumerable<UserBook>>(src.UserBooks);
                }

                var reviews = Enumerable.Empty<Review>();
                if (src.Reviews != null && src.Reviews.Count != 0)
                {
                    reviews = ctx.Mapper
                        .Map<IEnumerable<Review>>(src.Reviews);
                }

                return GeneralBook.Reconstitute(
                    src.Id,
                    src.Title,
                    src.Author,
                    src.Published,
                    languageCodeResult.Value,
                    photoResult.Value,
                    ratingAvg,
                    src.Genres ?? Array.Empty<BookGenre>(),

                    userBooks,
                    reviews
                );
            })
            .ForAllMembers(opt => opt.Ignore());

        // Domain -> Entity Mapping
        CreateMap<GeneralBook, GeneralBookEntity>(MemberList.Source)
            // map core scalar values
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.OriginalLanguage.Code))
            .ForMember(dest => dest.CoverPhoto, opt => opt.MapFrom(src => src.CoverPhoto.Link))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.ToList()))

            // ignore the RatingAvg *source* property so MemberList.Source is happy
            .ForSourceMember(src => src.RatingAvg, opt => opt.DoNotValidate())

             .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.UserReviews.ToList()))

            // explicitly IGNORE relations 
            // - not sure about that becouse when we map from domain entity we usually have full entity
            .ForMember(dest => dest.UserBooks, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistedByUsers, opt => opt.Ignore());
        
    }
}
