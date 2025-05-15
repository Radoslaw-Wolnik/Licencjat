using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapTimelineRepository
{
    Task<Result<Guid>> AddAsync(TimelineUpdate timelineUpdate, CancellationToken cancellationToken);
}