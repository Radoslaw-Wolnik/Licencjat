using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;


public class ListLibaryHandler
    : IRequestHandler<ListLibraryQuerry, Result<PaginatedResult<UserLibraryListItem>>>
{
    private readonly IUserBookQueryService _userBookQuery;
    private readonly IImageStorageService  _imageStorage;

    public ListLibaryHandler(
        IUserBookQueryService userQueryService,
        IImageStorageService imageStorageService)
    {
        _userBookQuery = userQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<PaginatedResult<UserLibraryListItem>>> Handle(
        ListLibraryQuerry request,
        CancellationToken cancellationToken)
    {
        var query = await _userBookQuery.ListLibraryAsync(request.UserId, request.NameFilter, request.AuthorFilter, request.SortBy, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(query);
        
        // mby change the picture keys for the public urls via imageService

    }
}