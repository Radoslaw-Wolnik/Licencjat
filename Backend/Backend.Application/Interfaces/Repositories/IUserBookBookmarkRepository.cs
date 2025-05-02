using Backend.Domain.Common;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserBookBookmarkRepository
{
    Task<IReadOnlyCollection<Bookmark>> GetByUserBookIdAsync(Guid userBookId);

    Task AddAsync(Bookmark bookmark);
    Task RemoveAsync(Guid bookmarkId);
    Task UpdateAsync(Bookmark bookmark);
}