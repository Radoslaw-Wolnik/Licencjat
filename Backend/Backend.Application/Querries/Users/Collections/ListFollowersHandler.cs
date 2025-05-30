using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

public class ListFollowersHandler
    : IRequestHandler<ListFollowersQuery, Result<PaginatedResult<UserSmallReadModel>>>
{
    private readonly IUserQueryService _userQuery;

    public ListFollowersHandler(IUserQueryService userQuery)
    {
        _userQuery = userQuery;
    }

    public async Task<Result<PaginatedResult<UserSmallReadModel>>> Handle(
        ListFollowersQuery request,
        CancellationToken cancellationToken)
    {
        var followers = await _userQuery.ListFollowersAsync(request.UserId, request.UsernameFilter, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(followers);
    }
}
