using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

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

    public async Task<IReadOnlyCollection<SocialMediaLink>> GetByUserIdAsync(Guid userId)
    {
        var entities = await _db.SocialMediaLinks
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
        return _mapper.Map<List<SocialMediaLink>>(entities);
    }

    public async Task AddAsync(SocialMediaLink link)
    {
        var entity = _mapper.Map<SocialMediaLinkEntity>(link);
        _db.SocialMediaLinks.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid linkId)
    {
        var existing = await _db.SocialMediaLinks.FindAsync(linkId);
        if (existing is null)
            throw new KeyNotFoundException($"SocialMediaLink with Id = {linkId} was not found.");

        _db.SocialMediaLinks.Remove(existing);
    }

    public async Task UpdateAsync(SocialMediaLink link)
    {
        var existing = await _db.SocialMediaLinks.FindAsync(link.Id);
        _mapper.Map(existing, link);
        await _db.SaveChangesAsync();
    }
    
}
