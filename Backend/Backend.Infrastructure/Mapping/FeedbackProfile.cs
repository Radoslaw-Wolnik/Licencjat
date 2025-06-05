using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class FeedbackProfile : Profile
{
    public FeedbackProfile()
    {
        CreateMap<FeedbackEntity, Feedback>()
            .ConstructUsing((src, _) =>
            {
                var result = Feedback.Create(src.Id, src.SubSwapId, src.UserId, src.Stars, src.Recommend, src.Lenght, src.ConditionBook, src.Communication);
                if (result.IsFailed)
                    throw new AutoMapperMappingException($"Invalid feedbck: {string.Join(", ", result.Errors)}");
                return result.Value;
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<Feedback, FeedbackEntity>()
            .ConstructUsing(src => new FeedbackEntity
            {
                Id = src.Id,
                SubSwapId = src.SubSwapId,
                UserId = src.UserId,
                Stars = src.Stars,
                Recommend = src.Recommend,
                Lenght = src.Length,
                ConditionBook = src.Condition,
                Communication = src.Communication
            });
    }
}