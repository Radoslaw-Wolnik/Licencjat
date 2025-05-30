using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

public sealed record ListWishlistQuery(
    Guid UserId,

    string? TitleFilter,

    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<BookCoverItemReadModel>>>;
