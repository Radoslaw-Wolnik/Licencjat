using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<ReviewEntity, Review>()
            .ConstructUsing(src => new Review(src.Id, src.UserId, src.BookId, src.Rating, src.CreatedAt, src.Comment))
            .ReverseMap();
    }
}