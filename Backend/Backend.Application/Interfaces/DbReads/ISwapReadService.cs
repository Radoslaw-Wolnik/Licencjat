using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface ISwapReadService
{
    Task<Swap?> GetByIdAsync(Guid swapId, CancellationToken cancellationToken = default);
    Task<Guid?> GetSubSwapId(Guid swapId, Guid userId, CancellationToken cancellationToken = default);
    Task<Meetup> GetMeetupById(Guid meetupId, CancellationToken cancellationToken = default);
}
