using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums.SortBy;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.GeneralBooks;

public sealed record ListReviewsQuerry(
    Guid GeneralBookId,

    SortReviewsBy SortBy,
    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<ReviewReadModel>>>;

// simillar to list the general books 
// but for the reviews
// with option to sort by
// and pagination