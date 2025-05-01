using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class MeetupProfile : Profile
{
    public MeetupProfile()
    {

        CreateMap<MeetupEntity, Meetup>(MemberList.None)
            .ConstructUsing((src, ctx) =>
            {

                LocationCoordinates? coords = null;
                if (src.Location_X == null || src.Location_Y == null)
                    coords = null;
                else {
                    var coordResult = LocationCoordinates.Create(src.Location_X.Value, src.Location_Y.Value);
                    if (coordResult.IsFailed)
                        throw new AutoMapperMappingException($"Bad location: {src.Location_X} {src.Location_Y}");
                    coords = coordResult.Value;
                }

                return new Meetup(
                    src.Id, 
                    src.SwapId, 
                    src.SuggestedUserId, 
                    src.Status,
                    coords);
            });
        
        CreateMap<Meetup, MeetupEntity>(MemberList.Source)
            .ForMember(dest => dest.Id,              opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SwapId,          opt => opt.MapFrom(src => src.SwapId))
            .ForMember(dest => dest.SuggestedUserId, opt => opt.MapFrom(src => src.SuggestedUserId))
            .ForMember(dest => dest.Status,          opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Location_X,      opt => opt.MapFrom(src => src.Location == null ? (double?)null : src.Location.Latitude))
            .ForMember(dest => dest.Location_Y,      opt => opt.MapFrom(src => src.Location == null ? (double?)null : src.Location.Longitude));
    }
}