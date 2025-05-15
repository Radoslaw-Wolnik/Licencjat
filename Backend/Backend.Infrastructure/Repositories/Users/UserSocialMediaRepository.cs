using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Repositories.Users;

public class UserSocialMediaRepository : IUserSocialMediaRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UserSocialMediaRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(SocialMediaLink link, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<SocialMediaLinkEntity>(link);
        _db.SocialMediaLinks.Add(entity);
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add SocialMediaLink");
        
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(SocialMediaLink link, CancellationToken cancellationToken)
    {
        var existing = await _db.SocialMediaLinks.FindAsync(link.Id);
        _mapper.Map(existing, link);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to updte SocialMediaLink");
    }

    public async Task<Result> RemoveAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var existing = await _db.SocialMediaLinks.FindAsync(linkId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("SocialMedaiLink", linkId));

        _db.SocialMediaLinks.Remove(existing);

        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to remove SocialMediaLink");
    }
}
