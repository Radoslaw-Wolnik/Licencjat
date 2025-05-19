using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Domain.Collections;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class Swap : Entity<Guid>
{
    public SubSwap SubSwapRequesting { get; }
    public SubSwap SubSwapAccepting { get; }

    private readonly MeetupsCollection _meetups;
    private readonly TimelineUpdatesCollection _timelineUpdates;

    public IReadOnlyCollection<Meetup> Meetups => _meetups.Meetups;
    public IReadOnlyCollection<TimelineUpdate> TimelineUpdates => _timelineUpdates.TimelineUpdates;

    // initial creation 
    private Swap(
        Guid requestingUserId, 
        UserBook requestingBook, 
        Guid acceptingUserId
    ) : this (
        Guid.NewGuid(), 
        SubSwap.Initial(requestingUserId, requestingBook), 
        SubSwap.Initial(acceptingUserId, null),
        initialMeetups: Enumerable.Empty<Meetup>(),
        InitialTimelineUpdates: []
    ) { }

    // reconstruction - main constructor
    private Swap(
        Guid id, 
        SubSwap subSwapRequesting, 
        SubSwap subSwapAccepting, 
        IEnumerable<Meetup> initialMeetups, 
        IEnumerable<TimelineUpdate> InitialTimelineUpdates
    ) {
        Id = id;
        SubSwapAccepting = subSwapAccepting;
        SubSwapRequesting = subSwapRequesting;
        _meetups = new MeetupsCollection(initialMeetups);
        _timelineUpdates = new TimelineUpdatesCollection(InitialTimelineUpdates);
    }

    public static Result<Swap> Create(Guid requestingUserId, UserBook requestingBook, Guid acceptingUserId)
    {
        if (requestingUserId == Guid.Empty)
            return Result.Fail(DomainErrorFactory.NotFound("swap.userRequesting", requestingUserId));
        if (acceptingUserId == Guid.Empty)
            return Result.Fail(DomainErrorFactory.NotFound("swap.userAccepting", acceptingUserId));

        return Result.Ok(new Swap(requestingUserId, requestingBook, acceptingUserId));
    }

    public static Swap Reconstitute(
        Guid id,
        SubSwap subSwapRequesitng,
        SubSwap subSwapAccepting,
        IEnumerable<Meetup> meetups,
        IEnumerable<TimelineUpdate> timelineUpdates
    ) {
        var swap = new Swap(id, subSwapRequesitng, subSwapAccepting, meetups, timelineUpdates);

        return swap;
    }

    public Result AddMeetup(Meetup meetup)
        => _meetups.Add(meetup);
    
    public Result RemoveMeetup(Guid meetupId)
        => _meetups.Remove(meetupId);
    
    public Result UpdateMeetup(Meetup updatedMeetup)
        => _meetups.Update(updatedMeetup);
    
    public Result AddTimelineUpdate(TimelineUpdate timelineUpdate)
        => _timelineUpdates.Add(timelineUpdate);
    
    public Result RemoveTimelineUpdate(Guid timelineUpdateId)
        => _timelineUpdates.Remove(timelineUpdateId);

    // ------------------ Subswap logic ------------------

    public Result InitialBookReading(Guid userId, UserBook userBook){
        if ( SubSwapAccepting.UserId != userId)
            return Result.Fail(DomainErrorFactory.Forbidden("Swap.SetUserBook", "Only person accepting the swap can set the book they want to read"));
        
        return SubSwapAccepting.InitialBook(userBook);
    }

    public Result UpdatePageReading(Guid userId, int page)
    {
        var subSwap = GetSubSwapByUserId(userId);
        if (subSwap is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap", userId));

        return subSwap.UpdatePageAt(page);
    }

    public Result AddIssue(Guid userId, Issue issue){
        var subSwap = GetSubSwapByUserId(userId);
        if (subSwap is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap.Issue", userId));

        return subSwap.AddIssue(issue);
    }

    public Result UpdateIssue(Guid userId, Issue updatedIssue){
        var subSwap = GetSubSwapByUserId(userId);
        if (subSwap is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap.Issue", userId));

        return subSwap.UpdateIssue(updatedIssue);
    }

    public Result RemoveIssue(Guid userId){
        var subSwap = GetSubSwapByUserId(userId);
        if (subSwap is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap.Issue", userId));

        subSwap.RemoveIssue();
        return Result.Ok();
    }

    public Result AddFeedback(Guid userId, Feedback feedback){
        var subSwap = GetSubSwapByUserId(userId);
        if (subSwap is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap.Feedback", userId));

        return subSwap.AddFeedback(feedback);
    }

    public Result UpdateFeedback(Guid userId, Feedback updatedFeedback){
        var subSwap = GetSubSwapByUserId(userId);
        if (subSwap is null)
            return Result.Fail(DomainErrorFactory.NotFound("Swap.Feedback", userId));

        return subSwap.UpdateFeedback(updatedFeedback);
    }
    

    private SubSwap? GetSubSwapByUserId(Guid userId) {
        if (SubSwapAccepting.UserId == userId)
            return SubSwapAccepting;
        if (SubSwapRequesting.UserId == userId)
            return SubSwapRequesting;
        return null;
    }

    public Guid? GetUserSubSwapId(Guid userId) {
        var subSwap = GetSubSwapByUserId(userId);
        return subSwap?.UserId;
    }
}