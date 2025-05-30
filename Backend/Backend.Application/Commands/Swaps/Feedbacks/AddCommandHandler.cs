using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Common;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Factories;


namespace Backend.Application.Commands.Swaps.Feedbacks;
public class AddFeedbackCommandHandler
    : IRequestHandler<AddFeedbackCommand, Result<Guid>>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly ISwapReadService _swapRead;

    public AddFeedbackCommandHandler(
        IWriteSwapRepository swapRepo,
        ISwapReadService swapReadService)
    {
        _swapRepo = swapRepo;
        _swapRead = swapReadService;
    }

    public async Task<Result<Guid>> Handle(
        AddFeedbackCommand request,
        CancellationToken cancellationToken)
    {
        var feedbackId = Guid.NewGuid();
        var subSwapId = await _swapRead.GetSubSwapId(request.SwapId, request.UserId, cancellationToken);
        if (subSwapId == null)
            return Result.Fail("Couldnt find the swap you want to add the feedbck to");

        // create new feedback
        var feedbackResult = Feedback.Create(feedbackId, (Guid)subSwapId, request.UserId, request.Stars, request.Recommend, request.Length, request.Condition, request.Communication);
        if (feedbackResult.IsFailed)
            return Result.Fail(feedbackResult.Errors);
        
        // save via repository root - swap
        var persistanceResult = await _swapRepo.AddFeedbackAsync(feedbackResult.Value, cancellationToken);
        if(persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);

        // add timeline update
        var updateResult = TimelineUpdateFactory.CreateCompleted(request.UserId, request.SwapId);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        await _swapRepo.AddTimelineUpdateAsync(updateResult.Value, cancellationToken);

        // here < -----------------
        // update user reputation (other one mby this as well) 
        // write code
         
        return Result.Ok(feedbackId);
    }
}
