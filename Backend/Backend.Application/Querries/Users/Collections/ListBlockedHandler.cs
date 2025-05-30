using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

// ListSocialMediaHandler.cs
public class ListBlockedHandler
    : IRequestHandler<ListBlockedQuery, Result<PaginatedResult<UserSmallReadModel>>>
{
    private readonly IUserQueryService _userQuery;

    public ListBlockedHandler(IUserQueryService userQuery)
    {
        _userQuery = userQuery;
    }

    public async Task<Result<PaginatedResult<UserSmallReadModel>>> Handle(
        ListBlockedQuery request,
        CancellationToken cancellationToken)
    {
        var blocked = await _userQuery.ListBlockedAsync(request.UserId, request.UsernameFilter, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(blocked);
    }
}
