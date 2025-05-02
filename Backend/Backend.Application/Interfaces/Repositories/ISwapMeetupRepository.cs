using Backend.Domain.Common;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapMeetupRepository
{
    Task<IReadOnlyCollection<Meetup>> GetByIdAsync(Guid swapId);

    Task AddAsync(Meetup meetup);
    Task RemoveAsync(Guid meetupId);
    Task UpdateAsync(Meetup meetup);
}