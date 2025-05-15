using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserBookBookmarkRepository
{
    Task<Result<Guid>> AddAsync(Bookmark bookmark, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid bookmarkId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Bookmark bookmark, CancellationToken cancellationToken);
}