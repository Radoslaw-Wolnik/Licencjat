using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Mapping;

public class GeneralBookReadModelProfile : Profile
{
    public GeneralBookReadModelProfile()
    {
        // GeneralBookEntity → GeneralBookListItem
        CreateMap<GeneralBookEntity, GeneralBookListItem>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Title", opt => opt.MapFrom(src => src.Title))
            .ForCtorParam("Author", opt => opt.MapFrom(src => src.Author))
            .ForCtorParam("CoverUrl", opt => opt.MapFrom(src => src.CoverPhoto))
            .ForCtorParam("RatingAvg", opt => opt.MapFrom(src =>
                                    src.Reviews.Any()
                                    ? src.Reviews.Average(r => r.Rating)
                                    : 0f))
            .ForCtorParam("PrimaryGenre", opt => opt.MapFrom(src =>
                                    src.Genres.Any()
                                    ? src.Genres.First()
                                    : (BookGenre?)null))
            // .ForCtorParam("PublicationDate", opt => opt.MapFrom(src => (DateOnly?)src.Published));
            .ForCtorParam("PublicationDate", opt => opt.MapFrom(src =>
                                    src.Published != DateOnly.MinValue
                                    ? (DateOnly?)src.Published
                                    : null));


        // GeneralBookEntity → GeneralBookDetailsReadModel
        CreateMap<GeneralBookEntity, GeneralBookDetailsReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("LanguageCode", opt => opt.MapFrom(src => src.Language))
            .ForCtorParam("CoverPhotoUrl", opt => opt.MapFrom(src => src.CoverPhoto))
            .ForCtorParam("RatingAvg", opt => opt.MapFrom(src =>
                src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 7)) // definitly should have made the avg nullable
            .ForCtorParam("Reviews", opt => opt.MapFrom((src, ctx) => src.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Take((int)ctx.Items["MaxReviews"])
                    .ToList()));

        // ReviewEntity → ReviewReadModel
        CreateMap<ReviewEntity, ReviewReadModel>()
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Rating", opt => opt.MapFrom(src => src.Rating))
            .ForCtorParam("Comment", opt => opt.MapFrom(src => src.Comment))
            .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt))
            .ForCtorParam("User", opt => opt.MapFrom(src => src.User ?? null));
            // but should throw error 
            // .ForCtorParam("User", opt => opt.MapFrom(src => src.User ?? throw new AutoMapperMappingException("Review has no User; cannot map ReviewReadModel"))); //  .ToUserSmallReadModel())); // use UserEntity → UserSmallReadModel mapping from UserReadModelProfile


    }
}