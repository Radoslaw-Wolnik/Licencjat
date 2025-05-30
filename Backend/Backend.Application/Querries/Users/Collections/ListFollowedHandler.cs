using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

public class ListFollowedHandler
    : IRequestHandler<ListFollowedQuery, Result<PaginatedResult<UserSmallReadModel>>>
{
    private readonly IUserQueryService _userQuery;

    public ListFollowedHandler(IUserQueryService userQuery)
    {
        _userQuery = userQuery;
    }

    public async Task<Result<PaginatedResult<UserSmallReadModel>>> Handle(
        ListFollowedQuery request,
        CancellationToken cancellationToken)
    {
        var followed = await _userQuery.ListFollowedAsync(request.UserId, request.UsernameFilter, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(followed);
    }
}
