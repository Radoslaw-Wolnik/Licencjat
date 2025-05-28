using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums.SortBy;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;

public sealed record ListLibraryQuerry(
    Guid UserId,
    string? NameFilter,
    string? AuthorFilter,

    SortUserBookBy SortBy,
    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<UserLibraryListItem>>>;
