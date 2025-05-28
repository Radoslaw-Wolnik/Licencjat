using AutoMapper;
using Backend.Domain.Entities;
using Backend.Domain.Common;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Mapping
{
    public class SwapProfile : Profile
    {
        public SwapProfile()
        {
            // Entity → Domain
            CreateMap<SwapEntity, Swap>()
                .ConstructUsing((src, ctx) =>
                {
                    // map your two SubSwaps
                    var requesting = ctx.Mapper.Map<SubSwap>(src.SubSwapRequesting);
                    var accepting  = ctx.Mapper.Map<SubSwap>(src.SubSwapAccepting);

                    // map any existing meetups & timeline updates
                    var meetups = src.Meetups?
                        .Select(m => ctx.Mapper.Map<Meetup>(m))
                        .ToList() ?? new List<Meetup>();

                    var timeline = src.TimelineUpdates?
                        .Select(t => ctx.Mapper.Map<TimelineUpdate>(t))
                        .ToList() ?? new List<TimelineUpdate>();

                    // Rehydrate your aggregate
                    return Swap.Reconstitute(src.Id, src.Status, requesting, accepting, meetups, timeline, src.CreatedAt);
                });

            // Domain → Entity
            CreateMap<Swap, SwapEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.SubSwapRequestingId, opt => opt.MapFrom(src => src.SubSwapRequesting.Id ))
                .ForMember(dest => dest.SubSwapAcceptingId, opt => opt.MapFrom(src => src.SubSwapAccepting != null ? (Guid?) src.SubSwapAccepting.Id : null))
                
                // collections managed separately
                .ForMember(dest => dest.Meetups, opt => opt.Ignore())
                .ForMember(dest => dest.TimelineUpdates, opt => opt.Ignore());
        }
    }
}
