using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public class GetMeetupByIdHandler
    : IRequestHandler<GetMeetupByIdQuery, Result<MeetupReadModel>>
{
    private readonly ISwapQueryService _swapQuery;

    public GetMeetupByIdHandler(
        ISwapQueryService swapQueryService)
    {
        _swapQuery = swapQueryService;
    }

    public async Task<Result<MeetupReadModel>> Handle(
        GetMeetupByIdQuery request,
        CancellationToken cancellationToken)
    {
        var meetup = await _swapQuery.GetMeetupByIdAsync(
            request.MeetupId, cancellationToken);

        if (meetup is null)
        {
            return Result.Fail(DomainErrorFactory.NotFound("Meetup", request.MeetupId));
        }

        return Result.Ok(meetup);
    }

}