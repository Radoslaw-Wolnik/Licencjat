using Backend.Domain.Common;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Enums.SortBy;
using System.Linq.Expressions;
using Backend.Application.ReadModels.UserBooks;

namespace Backend.Application.Interfaces.Queries;

public interface IUserBookQueryService
{

    Task<PaginatedResult<UserBookListItem>> ListAsync(
        Guid GeneralBookId, // list by general book

        SortUserBookBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default
    );

    Task<PaginatedResult<UserLibraryListItem>> ListLibraryAsync(
        Guid UserId, // list user library
        string? nameFiletr,
        string? authorFilter,

        SortUserBookBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default
    );

    Task<UserBookDetailsReadModel?> GetBookDetailsAsync(
        Guid bookId,
        CancellationToken ct = default
    );

    Task<UserOwnBookProfileReadModel?> GetOwnBookDetailsAsync(
        Guid bookId,
        int maxBookamrks = 10,
        CancellationToken ct = default
    );

    Task<PaginatedResult<BookmarkReadModel>> ListBookmarksAsync(
        Guid bookId,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);

    Task<BookmarkReadModel?> GetBookmarkByIdAsync(Guid bookmarkId, CancellationToken ct = default);
}
