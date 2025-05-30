using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public class GetFeedbackByIdHandler
    : IRequestHandler<GetFeedbackByIdQuery, Result<FeedbackReadModel>>
{
    private readonly ISwapQueryService _swapQuery;

    public GetFeedbackByIdHandler(
        ISwapQueryService swapQueryService)
    {
        _swapQuery = swapQueryService;
    }

    public async Task<Result<FeedbackReadModel>> Handle(
        GetFeedbackByIdQuery request,
        CancellationToken cancellationToken)
    {
        var feedback = await _swapQuery.GetFeedbackByIdAsync(
            request.FeedbackId, cancellationToken);

        if (feedback is null)
        {
            return Result.Fail(DomainErrorFactory.NotFound("Feedback", request.FeedbackId));
        }

        return Result.Ok(feedback);
    }

}