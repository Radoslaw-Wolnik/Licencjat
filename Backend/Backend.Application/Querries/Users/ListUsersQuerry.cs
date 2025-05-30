using Backend.Application.ReadModels.Common;
using Backend.Domain.Common;
using Backend.Domain.Enums.SortBy;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users;

public sealed record ListUsersQuerry(
    string? UserName,
    float? Reputation,

    string? City,
    string? Country,

    SortUsersBy SortBy,
    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<UserSmallReadModel>>>;
