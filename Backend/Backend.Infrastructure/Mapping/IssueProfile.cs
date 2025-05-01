using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Entities;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Mapping;

public class IssueProfile : Profile
{
    public IssueProfile()
    {
        CreateMap<IssueEntity, Issue>()
            .ConstructUsing(src => new Issue(src.Id, src.UserId, src.SubSwapId, src.Description))
            .ReverseMap();
    }
}