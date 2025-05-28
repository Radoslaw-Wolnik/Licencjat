using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Mapping;

public class GeneralBookReadModelProfile : Profile
{
    public GeneralBookReadModelProfile()
    {
        // GeneralBookEntity → GeneralBookListItem
        CreateMap<GeneralBookEntity, GeneralBookListItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src => src.CoverPhoto))
            .ForMember(dest => dest.RatingAvg, opt => opt.MapFrom(src => 
                src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
            .ForMember(dest => dest.PrimaryGenre, opt => opt.MapFrom(src => 
                src.Genres.FirstOrDefault()))
            .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => src.Published));

        // GeneralBookEntity → GeneralBookDetailsReadModel
        CreateMap<GeneralBookEntity, GeneralBookDetailsReadModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.CoverPhotoUrl, opt => opt.MapFrom(src => src.CoverPhoto))
            .ForMember(dest => dest.RatingAvg, opt => opt.MapFrom(src => 
                src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom((src, dest, _, ctx) => 
                src.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Take((int)ctx.Items["MaxReviews"])
                    .ToList()));

        // ReviewEntity → ReviewReadModel
        CreateMap<ReviewEntity, ReviewReadModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User)); // use UserEntity → UserSmallReadModel mapping from UserReadModelProfile
    }
}