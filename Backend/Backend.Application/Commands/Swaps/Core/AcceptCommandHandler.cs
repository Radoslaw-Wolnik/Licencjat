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
public class AcceptSwapCommandHandler
    : IRequestHandler<AcceptSwapCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly ISwapReadService _swapRead;
    private readonly IUserBookReadService _bookRead;

    public AcceptSwapCommandHandler(
        IWriteSwapRepository swapRepository,
        ISwapReadService swapReadService,
        IUserBookReadService userBookReadService)
    {
        _swapRepo = swapRepository;
        _swapRead = swapReadService;
        _bookRead = userBookReadService;
    }

    public async Task<Result> Handle(
        AcceptSwapCommand request,
        CancellationToken cancellationToken)
    {
        // fetch the swap
        var swap = await _swapRead.GetByIdAsync(request.SwapId, cancellationToken);
        if (swap == null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap", request.SwapId));

        // fetch the book user wants to read
        var book = await _bookRead.GetByIdAsync(request.RequestedBookId, cancellationToken);
        if (book == null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBook", request.RequestedBookId));

        // accept the swap
        swap.InitialBookReading(request.UserAcceptingId, book);

        // persist changes
        var persistanceResult = await _swapRepo.UpdateAsync(swap, cancellationToken);

        // add timeline update
        var updateResult = TimelineUpdateFactory.CreateResponse(request.UserAcceptingId, swap.Id, true);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        await _swapRepo.AddTimelineUpdateAsync(updateResult.Value, cancellationToken);

        return persistanceResult;
    }
}
