using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Errors;
using Backend.Domain.Factories;

namespace Backend.Application.Commands.Swaps.Issues;

public class RemoveIssueCommandHandler
    : IRequestHandler<RemoveIssueCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;

    public RemoveIssueCommandHandler(
        IWriteSwapRepository swapRepo)
    {
        _swapRepo = swapRepo;
    }

    public async Task<Result> Handle(
        RemoveIssueCommand request,
        CancellationToken cancellationToken)
    {
        // add timeline update
        var updateResult = TimelineUpdateFactory.CreateResolved(request.UserId, request.SwapId, request.ResolutionDetails);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        await _swapRepo.AddTimelineUpdateAsync(updateResult.Value, cancellationToken);
        
        return await _swapRepo.RemoveIssueAsync(request.IssueId, cancellationToken);
    }
}
