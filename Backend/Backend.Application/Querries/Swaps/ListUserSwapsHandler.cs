using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;


public class ListUserSwapsHandler
    : IRequestHandler<ListUserSwapsQuerry, Result<PaginatedResult<SwapListItem>>>
{
    private readonly ISwapQueryService _swapQuery;
    private readonly IImageStorageService  _imageStorage;

    public ListUserSwapsHandler(
        ISwapQueryService swapQueryService,
        IImageStorageService imageStorageService)
    {
        _swapQuery = swapQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<PaginatedResult<SwapListItem>>> Handle(
        ListUserSwapsQuerry request,
        CancellationToken cancellationToken)
    {
        var query = await _swapQuery.ListAsync(request.UserId, request.Status, request.Descending, request.Offset, request.Limit, cancellationToken);

        return Result.Ok(query);
        
        // mby change the profileurls for the official ones

    }
}