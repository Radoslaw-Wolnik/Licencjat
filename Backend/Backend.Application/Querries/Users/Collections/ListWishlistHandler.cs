using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

// ListSocialMediaHandler.cs
public class ListWishlistHandler
    : IRequestHandler<ListWishlistQuery, Result<PaginatedResult<BookCoverItemReadModel>>>
{
    private readonly IUserQueryService _userQuery;

    public ListWishlistHandler(IUserQueryService userQuery)
    {
        _userQuery = userQuery;
    }

    public async Task<Result<PaginatedResult<BookCoverItemReadModel>>> Handle(
        ListWishlistQuery request,
        CancellationToken cancellationToken)
    {
        var wishlist = await _userQuery.ListWishlistAsync(request.UserId, request.TitleFilter, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(wishlist);
    }
}
