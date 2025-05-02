using Backend.Domain.Common;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapTimelineRepository
{
    Task<IReadOnlyCollection<TimelineUpdate>> GetByIdAsync(Guid swapId);

    Task AddAsync(TimelineUpdate timelineUpdate);
}