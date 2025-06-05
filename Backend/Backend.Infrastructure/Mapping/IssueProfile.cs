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
            .ConstructUsing((src, _) =>
            {
                var result = Issue.Create(src.Id, src.UserId, src.SubSwapId, src.Description);
                if (result.IsFailed)
                    throw new AutoMapperMappingException($"Invalid issue: {string.Join(", ", result.Errors)}");
                return result.Value;
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<Issue, IssueEntity>()
            .ConstructUsing(src => new IssueEntity
            {
                Id = src.Id,
                UserId = src.UserId,
                SubSwapId = src.SubSwapId,
                Description = src.Description
            });
    }
}