using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapTimelineRepository
{
    Task<IReadOnlyCollection<TimelineUpdate>> GetByIdAsync(Guid swapId);

    Task<Result<Guid>> AddAsync(TimelineUpdate timelineUpdate, CancellationToken cancellationToken);
}