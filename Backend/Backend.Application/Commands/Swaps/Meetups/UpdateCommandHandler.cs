using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Application.Commands.Swaps.Meetups;
public class UpdateMeetupCommandHandler
    : IRequestHandler<UpdateMeetupCommand, Result<Meetup>>
{
    private readonly IWriteSwapRepository _swapRepo;
    private readonly ISwapReadService _swapRead;

    public UpdateMeetupCommandHandler(
        IWriteSwapRepository swapRepo,
        ISwapReadService swapReadService
    ) {
        _swapRepo = swapRepo;
        _swapRead = swapReadService;
    }

    public async Task<Result<Meetup>> Handle(
        UpdateMeetupCommand request,
        CancellationToken cancellationToken
    )
    {
        // load the exsisting - previous meetup
        var existing = await _swapRead.GetMeetupById(request.MeetupId, cancellationToken);
        if (existing == null)
            return Result.Fail("Meetup not found");

        var newLocation = existing.Location;
        if (request.Latitude != null && request.Longitude != null)
        {
            var locationResult = LocationCoordinates.Create((double)request.Latitude, (double)request.Longitude);
            if (locationResult.IsFailed)
                return Result.Fail(locationResult.Errors);
            newLocation = locationResult.Value;
        }

        // logic
        if (existing.Status == MeetupStatus.Completed)
            return Result.Fail("Cant chnage meetup taht already happended");

        if (existing.Status == MeetupStatus.Proposed || existing.Status == MeetupStatus.ChangedLocation)
            if (request.Status != MeetupStatus.Confirmed || request.Status != MeetupStatus.ChangedLocation)
                return Result.Fail("After proposing meetup can only change location or accept");

        if (request.Status == MeetupStatus.ChangedLocation && newLocation == existing.Location)
            return Result.Fail("Cant change location to the same one as the previous location for meetup");
        

        var createResult = Meetup.Create(
            id: existing.Id,
            swapId: existing.SwapId,
            suggestedUserId: existing.SuggestedUserId,
            status: request.Status,
            location: newLocation
        );

        if (createResult.IsFailed)
            return Result.Fail(createResult.Errors);

        // persistance save
        var persistance = await _swapRepo.UpdateMeetupAsync(createResult.Value, cancellationToken);
        if (persistance.IsFailed)
            return Result.Fail(persistance.Errors);

        return Result.Ok(createResult.Value);
    }
}
