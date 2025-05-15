using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapMeetupRepository
{
    Task<Result<Guid>> AddAsync(Meetup meetup, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid meetupId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Meetup meetup, CancellationToken cancellationToken);
}