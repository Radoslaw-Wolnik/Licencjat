using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Factories;

namespace Backend.Application.Commands.Swaps.Core;
public class DenySwapCommandHandler
    : IRequestHandler<DenySwapCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly ISwapReadService _swapRead;

    public DenySwapCommandHandler(
        IWriteSwapRepository swapRepository,
        ISwapReadService swapReadService)
    {
        _swapRepo = swapRepository;
        _swapRead = swapReadService;
    }

    public async Task<Result> Handle(
        DenySwapCommand request,
        CancellationToken cancellationToken)
    {
        // fetch the swap
        var swap = await _swapRead.GetByIdAsync(request.SwapId, cancellationToken);
        if (swap == null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap", request.SwapId));

        // persist changes
        var persistanceResult = await _swapRepo.UpdateAsync(swap, cancellationToken);

        // add timeline update
        var updateResult = TimelineUpdateFactory.CreateResponse(request.UserId, swap.Id, false);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        await _swapRepo.AddTimelineUpdateAsync(updateResult.Value, cancellationToken);

        return persistanceResult;
        
    }
}
