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
            .ConstructUsing((src, _) =>
            {
                var result = Review.Create(src.Id, src.UserId, src.BookId, src.Rating, src.CreatedAt, src.Comment);
                if (result.IsFailed)
                    throw new AutoMapperMappingException($"Invalid review: {string.Join(", ", result.Errors)}");
                return result.Value;
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<Review, ReviewEntity>()
            .ConstructUsing(src => new ReviewEntity
            {
                Id = src.Id,
                UserId = src.UserId,
                BookId = src.BookId,
                Rating = src.Rating,
                CreatedAt = src.CreatedAt,
                Comment = src.Comment
            });
    }
}