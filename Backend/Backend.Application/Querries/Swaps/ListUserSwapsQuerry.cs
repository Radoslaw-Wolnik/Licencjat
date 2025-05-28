using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public sealed record ListUserSwapsQuerry(
    Guid UserId,

    SwapStatus Status,
    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<SwapListItem>>>;
