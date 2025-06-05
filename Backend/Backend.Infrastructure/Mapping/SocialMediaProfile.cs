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
            .ConstructUsing((src, _) =>
            {
            var result = SocialMediaLink.Create(src.Id, src.Platform, src.Url);
                if (result.IsFailed)
                    throw new AutoMapperMappingException($"Invalid social media link: {string.Join(", ", result.Errors)}");
                return result.Value;
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<SocialMediaLink, SocialMediaLinkEntity>()
            .ConstructUsing(src => new SocialMediaLinkEntity { Id = src.Id, Platform = src.Platform, Url = src.Url });
    }

}