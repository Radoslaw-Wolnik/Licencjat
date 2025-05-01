using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Repositories.Swaps;

public interface ISwapMeetupRepository
{
    Task<IReadOnlyCollection<Meetup>> GetByIdAsync(Guid swapId);

    Task AddAsync(Meetup meetup);
    Task RemoveAsync(Guid meetupId);
    Task UpdateAsync(Meetup meetup);
}


public class SwapMeetupRepository : ISwapMeetupRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public SwapMeetupRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Meetup>> GetByIdAsync(Guid swapId)
    {
        var entities = await _db.Meetups
            .AsNoTracking()
            .Where(x => x.SwapId == swapId)
            .ToListAsync();
        return _mapper.Map<List<Meetup>>(entities);
    }

    public async Task AddAsync(Meetup meetup)
    {
        var entity = _mapper.Map<MeetupEntity>(meetup);
        _db.Meetups.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid meetupId)
    {
        var existing = await _db.Meetups.FindAsync(meetupId);
        if (existing is null)
            throw new KeyNotFoundException($"Meetup with Id = {meetupId} was not found.");

        _db.Meetups.Remove(existing);
    }

    public async Task UpdateAsync(Meetup meetup)
    {
        var existing = await _db.Meetups.FindAsync(meetup.Id);
        _mapper.Map(existing, meetup);
        await _db.SaveChangesAsync();
    }
    
}
