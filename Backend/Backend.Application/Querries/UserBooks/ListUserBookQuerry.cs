using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums.SortBy;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;

public sealed record ListUserBooksQuerry(
    Guid GeneralBookId,

    SortUserBookBy SortBy,
    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<UserBookListItem>>>;
