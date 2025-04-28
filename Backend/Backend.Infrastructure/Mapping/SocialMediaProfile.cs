using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Entities;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Application.DTOs;
using FluentResults;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Mapping;

public class SocialMediaProfile : Profile
{
    public SocialMediaProfile()
    {
        CreateMap<SocialMediaLinkEntity, SocialMediaLink>()
            .ConstructUsing(src => new SocialMediaLink(src.Id, src.Platform, src.Url))
            .ReverseMap();
            // what about user id in the socialMEdiaEntity? Would it be taken by the owner.Id? 
    }

}