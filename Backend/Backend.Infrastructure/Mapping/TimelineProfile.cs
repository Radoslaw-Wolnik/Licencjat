using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class TimelineProfile : Profile
{
    public TimelineProfile()
    {
        CreateMap<TimelineEntity, TimelineUpdate>()
            .ConstructUsing(src => new TimelineUpdate(src.Id, src.UserId, src.Status, src.Description))
            .ReverseMap();
    }
}