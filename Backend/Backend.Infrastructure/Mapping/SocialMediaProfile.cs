using AutoMapper;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class SocialMediaProfile : Profile
{
    public SocialMediaProfile()
    {
        CreateMap<SocialMediaLinkEntity, SocialMediaLink>()
            .ConstructUsing(src => new SocialMediaLink(src.Id, src.Platform, src.Url))
            .ReverseMap();
    }

}