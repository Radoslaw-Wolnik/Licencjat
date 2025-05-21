using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Common;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Enums;
using Backend.Domain.Factories;


namespace Backend.Application.Commands.Swaps.Meetups;
public class AddMeetupCommandHandler
    : IRequestHandler<AddMeetupCommand, Result>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly ISwapReadService _swapRead;

    public AddMeetupCommandHandler(
        IWriteSwapRepository swapRepo,
        ISwapReadService swapReadService)
    {
        _swapRepo = swapRepo;
        _swapRead = swapReadService;
    }

    public async Task<Result> Handle(
        AddMeetupCommand request,
        CancellationToken cancellationToken)
    {
        // check if previous meetups are completed - if not the new one cant be added;
        var swap = await _swapRead.GetByIdAsync(request.SwapId, cancellationToken);
        if (swap == null)
            return Result.Fail("Cant add meetup to swap that doesnt exists");

        var lastMeetup = swap.Meetups.Last();
        if (lastMeetup.Status != MeetupStatus.Completed)
            return Result.Fail("Cant add enw meetup if prev one is not completed");


        var meetupId = Guid.NewGuid();
        // create the location
        var locationResult = LocationCoordinates.Create(request.Latitude, request.Longitude);
        if (locationResult.IsFailed)
            return Result.Fail(locationResult.Errors);

        // create new meetup
        var meetupResult = Meetup.Create(meetupId, request.SwapId, request.UserId, MeetupStatus.Proposed, locationResult.Value);
        if (meetupResult.IsFailed)
            return Result.Fail(meetupResult.Errors);
        
        // save via repository root - swap
        var persistanceResult = await _swapRepo.AddMeetupAsync(meetupResult.Value, cancellationToken);
        if (persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);


        // add timeline update
        var updateResult = TimelineUpdateFactory.CreateMeetingUp(request.UserId, request.SwapId);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        await _swapRepo.AddTimelineUpdateAsync(updateResult.Value, cancellationToken);

        return Result.Ok();
    }
}
