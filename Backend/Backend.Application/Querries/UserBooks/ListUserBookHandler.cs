using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;


public class ListUserBookHandler
    : IRequestHandler<ListUserBooksQuerry, Result<PaginatedResult<UserBookListItem>>>
{
    private readonly IUserBookQueryService _userBookQuery;
    private readonly IImageStorageService  _imageStorage;

    public ListUserBookHandler(
        IUserBookQueryService userQueryService,
        IImageStorageService imageStorageService)
    {
        _userBookQuery = userQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<PaginatedResult<UserBookListItem>>> Handle(
        ListUserBooksQuerry request,
        CancellationToken cancellationToken)
    {
        var query = await _userBookQuery.ListAsync(request.GeneralBookId, request.SortBy, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(query);
        
        // mby change the picture keys for the public urls via imageService

    }
}