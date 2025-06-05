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
public class CreateSwapCommandHandler
    : IRequestHandler<CreateSwapCommand, Result<Guid>>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly IUserBookReadService _bookRead;

    public CreateSwapCommandHandler(
        IWriteSwapRepository swapRepository,
        IUserBookReadService userBookReadService)
    {
        _swapRepo = swapRepository;
        _bookRead = userBookReadService;
    }

    public async Task<Result<Guid>> Handle(
        CreateSwapCommand request,
        CancellationToken cancellationToken)
    {
        // fetch the book user wants to read
        var book = await _bookRead.GetByIdAsync(request.RequestedBookId, cancellationToken);
        if (book == null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBook", request.RequestedBookId));

        var timeNow = DateOnly.FromDateTime(DateTime.UtcNow);
        // create swap
        var swapResult = Swap.Create(request.UserRequestingId, book, request.UserAcceptingId);
        var swapResult = Swap.Create(request.UserRequestingId, book, request.UserAcceptingId, timeNow);
        if (swapResult.IsFailed)
            return Result.Fail(swapResult.Errors);

        // persist
        var persistanceResult = await _swapRepo.AddAsync(swapResult.Value, cancellationToken);
        if (persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);

        // add timeline update
        var updateResult = TimelineUpdateFactory.CreateRequested(request.UserRequestingId, swapResult.Value.Id);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        await _swapRepo.AddTimelineUpdateAsync(updateResult.Value, cancellationToken);

        return Result.Ok(swapResult.Value.Id);
    }
}
