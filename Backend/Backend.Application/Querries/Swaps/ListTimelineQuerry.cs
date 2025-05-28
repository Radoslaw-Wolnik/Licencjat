using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public sealed record ListTimelineQuerry(
    Guid SwapId,

    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<TimelineUpdateReadModel>>>;
