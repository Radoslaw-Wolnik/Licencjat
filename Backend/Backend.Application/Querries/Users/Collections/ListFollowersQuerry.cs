using Backend.Application.ReadModels.Common;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

public sealed record ListFollowersQuery(
    Guid UserId,

    string? UsernameFilter,

    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<UserSmallReadModel>>>;
    // just username + profile picture
