using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Repositories.Swaps;

public class SwapMeetupRepository : ISwapMeetupRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public SwapMeetupRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(Meetup meetup, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<MeetupEntity>(meetup);
        _db.Meetups.Add(entity);
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add Meetup");
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(Meetup meetup, CancellationToken cancellationToken)
    {
        var existing = await _db.Meetups.FindAsync(meetup.Id);
        _mapper.Map(existing, meetup);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update Meetup");
    }

    public async Task<Result> RemoveAsync(Guid meetupId, CancellationToken cancellationToken)
    {
        var existing = await _db.Meetups.FindAsync(meetupId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Meetup", meetupId));

        _db.Meetups.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to remove Meetup");
    }
    
}
