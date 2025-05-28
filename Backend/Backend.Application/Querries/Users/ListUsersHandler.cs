using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users;


public class ListUsersHandler
    : IRequestHandler<ListUsersQuerry, Result<PaginatedResult<UserSmallReadModel>>>
{
    private readonly IUserQueryService _userQuery;
    private readonly IImageStorageService  _imageStorage;

    public ListUsersHandler(
        IUserQueryService userQueryService,
        IImageStorageService imageStorageService)
    {
        _userQuery = userQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<PaginatedResult<UserSmallReadModel>>> Handle(
        ListUsersQuerry request,
        CancellationToken cancellationToken)
    {
        var query = await _userQuery.ListAsync(request.UserName, request.Reputation, request.City, request.Country, request.SortBy, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(query);
        
        // mby change the profileurls for the official ones

    }
}