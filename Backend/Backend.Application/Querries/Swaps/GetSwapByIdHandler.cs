using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Swaps;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;


public class GetSwapByIdHandler
    : IRequestHandler<GetSwapByIdQuerry, Result<SwapDetailsReadModel?>>
{
    private readonly ISwapQueryService _swapQuery;
    private readonly IImageStorageService  _imageStorage;

    public GetSwapByIdHandler(
        ISwapQueryService swapQueryService,
        IImageStorageService imageStorageService)
    {
        _swapQuery = swapQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<SwapDetailsReadModel?>> Handle(
        GetSwapByIdQuerry request,
        CancellationToken cancellationToken)
    {
        var query = await _swapQuery.GetDetailsAsync(request.SwapId, 10, cancellationToken);

        return Result.Ok(query);
        
        // mby change the profileurls for the official ones

    }
}