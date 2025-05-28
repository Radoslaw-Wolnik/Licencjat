using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Mapping;

public class TimelineProfile : Profile
{
    public TimelineProfile()
    {
        CreateMap<TimelineEntity, TimelineUpdate>()
            .ConstructUsing(src => new TimelineUpdate(src.Id, src.UserId, src.SwapId, src.Status, src.Description, src.CreatedAt));

        CreateMap<TimelineUpdate, TimelineEntity>(MemberList.Source)
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.SwapId, opt => opt.MapFrom(src => src.SwapId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<TimelineStatus, SwapStatus>()
            .ConvertUsing<TimelineStatusToSwapStatusConverter>();
    }
}
