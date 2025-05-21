using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Common;
using Backend.Application.Interfaces.DbReads;


namespace Backend.Application.Commands.Swaps.Feedbacks;
public class AddFeedbackCommandHandler
    : IRequestHandler<AddFeedbackCommand, Result>
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

    public async Task<Result> Handle(
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
        // or add in in the repo function
        

        return Result.Ok();
    }
}
