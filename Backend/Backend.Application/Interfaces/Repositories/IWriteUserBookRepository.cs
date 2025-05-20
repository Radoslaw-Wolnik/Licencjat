using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteUserBookRepository
{
    Task<Result<Guid>> AddAsync(UserBook book, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken);
    Task<Result> UpdateScalarsAsync(UserBook book, CancellationToken cancellationToken);

    // update bookmark collection
    Task<Result> UpdateBookmarksAsync(Guid bookId, IEnumerable<Bookmark> domainBookmarks, CancellationToken cancellationToken);
    Task<Result> AddBookmarkAsync(Bookmark bookmark, CancellationToken cancellationToken);
    Task<Result> RemoveBookmarkAsync(Guid bookmarkId, CancellationToken cancellationToken);
    Task<Result> UpdateBookmarkAsync(Bookmark updated, CancellationToken cancellationToken);

}