using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Mapping;

public class SubSwapProfile : Profile
{
    public SubSwapProfile()
    {
        // Entity → Domain
        CreateMap<SubSwapEntity, SubSwap>()
            .ConstructUsing((src, ctx) =>
            {
                var result = SubSwap.Create(
                    src.Id,
                    src.UserId,
                    src.PageAt,
                    src.UserBookReadingId,
                    src.FeedbackId,
                    src.IssueId
                );
                if (result.IsFailed)
                    throw new AutoMapperMappingException(
                        $"Error mapping SubSwapEntity {src.Id}: {string.Join(", ", result.Errors)}"
                    );
                return result.Value;
            });

        // Domain → Entity
        CreateMap<SubSwap, SubSwapEntity>()
            .ForMember(dest => dest.Id,                 opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId,             opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.PageAt,             opt => opt.MapFrom(src => src.PageAt))
            .ForMember(dest => dest.UserBookReadingId,  opt => opt.MapFrom(src => src.UserBookReadingId))
            .ForMember(dest => dest.FeedbackId,         opt => opt.MapFrom(src => src.FeedbackId))
            .ForMember(dest => dest.IssueId,            opt => opt.MapFrom(src => src.IssueId));
    }
}
