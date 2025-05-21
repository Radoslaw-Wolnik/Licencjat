using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Common;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Factories;


namespace Backend.Application.Commands.Swaps.Issues;
public class AddIssueCommandHandler
    : IRequestHandler<AddIssueCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly ISwapReadService _swapRead;

    public AddIssueCommandHandler(
        IWriteSwapRepository swapRepo,
        ISwapReadService swapReadService)
    {
        _swapRepo = swapRepo;
        _swapRead = swapReadService;
    }

    public async Task<Result> Handle(
        AddIssueCommand request,
        CancellationToken cancellationToken)
    {
        var issueId = Guid.NewGuid();
        var subSwapId = await _swapRead.GetSubSwapId(request.SwapId, request.UserId, cancellationToken);
        if (subSwapId == null)
            return Result.Fail("Couldnt find the swap you want to add the issue to");

        // create new issue
        var issueResult = Issue.Create(issueId, request.UserId, (Guid)subSwapId, request.Description);
        if (issueResult.IsFailed)
            return Result.Fail(issueResult.Errors);
        
        // save via repository root - swap
        var persistanceResult = await _swapRepo.AddIssueAsync(issueResult.Value, cancellationToken);
        if(persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);

        // add timeline update
        var updateResult = TimelineUpdateFactory.CreateDispute(request.UserId, request.SwapId, issueResult.Value.Description);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        await _swapRepo.AddTimelineUpdateAsync(updateResult.Value, cancellationToken);

        return Result.Ok();
    }
}
