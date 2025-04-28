using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories.Users;

// Social‑media (rich nav)
// change to use automapper <-------------------------------- <---
public interface IUserSocialMediaRepository
{
    Task<Result<IReadOnlyCollection<SocialMediaLink>>> GetSocialMediaAsync(Guid userId);

    Task<Result> AddSocialMediaAsync(Guid userId, SocialMediaLink link);
    Task<Result> RemoveSocialMediaAsync(Guid userId, Guid linkId);
    Task<Result> UpdateSocialMediaAsync(Guid userId, SocialMediaLink updated);
    
    Task<Result<bool>> SocialMediaContainsAsync(Guid userId, Guid bookId);
}


public class UserSocialMediaRepository : IUserSocialMediaRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserSocialMediaRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<SocialMediaLink>>> GetSocialMediaAsync(Guid userId)
    {
        var data = await _context.SocialMediaLinks
            .Where(s => s.UserId == userId)
            // .Select(s => new { s.Platform, s.Url })
            .ToListAsync();
        
        var mappedList = _mapper.Map<IReadOnlyCollection<SocialMediaLink>>(data);

        return Result.Ok(mappedList);
    }

    public async Task<Result> AddSocialMediaAsync(Guid userId, SocialMediaLink link)
    {
        var user = await _context.Users
            .Include(u => u.SocialMediaLinks)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Result.Fail(UserErrors.NotFound);

        // add
        var entity = _mapper.Map<SocialMediaLinkEntity>(link);
        entity.UserId = userId;   // make sure the FK is set
        user.SocialMediaLinks.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result> RemoveSocialMediaAsync(Guid userId, Guid linkId)
    {
        var entity = await _context.SocialMediaLinks
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == linkId);
        if (entity is null) return Result.Fail(UserErrors.NotFound);

        _context.SocialMediaLinks.Remove(entity);

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    
    public async Task<Result> UpdateSocialMediaAsync(Guid userId, SocialMediaLink updated)
    {
        var user = await _context.Users
            .Include(u => u.SocialMediaLinks)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null) return Result.Fail(UserErrors.NotFound);
        
        var entity = user.SocialMediaLinks
            .SingleOrDefault(sm => sm.Id == updated.Id);

        if (entity is null)
            return Result.Fail(SocialMediaErrors.NotFound);

        // check duplicates among *the others* before you overwrite
        var others = user.SocialMediaLinks.Where(sm => sm.Id != updated.Id);

        if (others.Any(sm => sm.Platform == updated.Platform))
            return Result.Fail(SocialMediaErrors.PlatformAlreadyExists);

        if (others.Any(sm => sm.Url == updated.Url))
            return Result.Fail(SocialMediaErrors.UrlAlreadyExists);
        
        _mapper.Map(updated, entity);

        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result<bool>> SocialMediaContainsAsync(Guid userId, Guid socialMediaId)
    {
        var exists = await _context.SocialMediaLinks
            .AnyAsync(s => s.UserId == userId && s.Id == socialMediaId);

        return Result.Ok(exists);
    }

    
}
