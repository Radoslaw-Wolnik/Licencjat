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
            .ConstructUsing(src => new Feedback(src.Id, src.SubSwapId, src.UserId, src.Stars, src.Recommend, src.Lenght, src.ConditionBook, src.Communication))
            .ReverseMap();
    }
}