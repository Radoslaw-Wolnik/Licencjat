using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IUserBookReadService
{
    Task<UserBook> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserBook> GetByAsync(Expression<Func<BookProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<UserBook> GetFullByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // only for updting a bookmark
    Task<Bookmark> GetBookmarkByIdAsync(Guid bookmarkId, CancellationToken cancellationToken = default);
}
