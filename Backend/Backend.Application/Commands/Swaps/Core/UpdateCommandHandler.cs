using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Application.Commands.Swaps.Core;
public class UpdateSwapCommandHandler
    : IRequestHandler<UpdateSwapCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly ISwapReadService _swapRead;

    public UpdateSwapCommandHandler(
        IWriteSwapRepository swapRepository,
        ISwapReadService swapReadService)
    {
        _swapRepo = swapRepository;
        _swapRead = swapReadService;
    }

    public async Task<Result> Handle(
        UpdateSwapCommand request,
        CancellationToken cancellationToken)
    {
        // fetch the swap
        var swap = await _swapRead.GetByIdAsync(request.SwapId, cancellationToken);
        if (swap == null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap", request.SwapId));

        // accept the swap
        swap.UpdatePageReading(request.UserId, request.PageAt);

        // add timeline update


        // persist changes
        // var persistanceResult =
        return await _swapRepo.UpdateAsync(swap, cancellationToken);
    }
}
