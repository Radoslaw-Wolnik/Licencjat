using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;

public sealed record ListBookmarksQuerry(
    Guid UserBookId,

    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<BookmarkReadModel>>>;