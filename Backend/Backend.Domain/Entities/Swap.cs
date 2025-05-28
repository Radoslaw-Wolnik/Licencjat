using Backend.Domain.Common;
using Backend.Domain.Errors;
using Backend.Domain.Collections;
using FluentResults;
using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public sealed class Swap : Entity<Guid>
{
    public SwapStatus Status { get; private set; }
    public SubSwap SubSwapRequesting { get; }
    public SubSwap SubSwapAccepting { get; }

    private readonly MeetupsCollection _meetups;
    private readonly TimelineUpdatesCollection _timelineUpdates;

    public IReadOnlyCollection<Meetup> Meetups => _meetups.Meetups;
    public IReadOnlyCollection<TimelineUpdate> TimelineUpdates => _timelineUpdates.TimelineUpdates;
    public DateOnly CreatedAt { get; }

    // initial creation 
    private Swap(
        Guid requestingUserId,
        UserBook requestingBook,
        Guid acceptingUserId,
        DateOnly createdAt
    ) : this(
        Guid.NewGuid(),
        SwapStatus.Requested,
        SubSwap.Initial(requestingUserId, requestingBook),
        SubSwap.Initial(acceptingUserId, null),
        initialMeetups: Enumerable.Empty<Meetup>(),
        InitialTimelineUpdates: [],
        createdAt
    )
    { }

    // reconstruction - main constructor
    private Swap(
        Guid id, 
        SwapStatus status,
        SubSwap subSwapRequesting, 
        SubSwap subSwapAccepting, 
        IEnumerable<Meetup> initialMeetups, 
        IEnumerable<TimelineUpdate> InitialTimelineUpdates,
        DateOnly createdAt
    ) {
        Id = id;
        Status = status;
        SubSwapAccepting = subSwapAccepting;
        SubSwapRequesting = subSwapRequesting;
        _meetups = new MeetupsCollection(initialMeetups);
        _timelineUpdates = new TimelineUpdatesCollection(InitialTimelineUpdates);
        CreatedAt = createdAt;
    }

    public static Result<Swap> Create(Guid requestingUserId, UserBook requestingBook, Guid acceptingUserId, DateOnly createdAt)
    {
        if (requestingUserId == Guid.Empty)
            return Result.Fail(DomainErrorFactory.NotFound("swap.userRequesting", requestingUserId));
        if (acceptingUserId == Guid.Empty)
            return Result.Fail(DomainErrorFactory.NotFound("swap.userAccepting", acceptingUserId));

        return Result.Ok(new Swap(requestingUserId, requestingBook, acceptingUserId, createdAt));
    }

    public static Swap Reconstitute(
        Guid id,
        SwapStatus status,
        SubSwap subSwapRequesitng,
        SubSwap subSwapAccepting,
        IEnumerable<Meetup> meetups,
        IEnumerable<TimelineUpdate> timelineUpdates,
        DateOnly createdAt
    ) {
        var swap = new Swap(id, status, subSwapRequesitng, subSwapAccepting, meetups, timelineUpdates, createdAt);

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

    public void UpdateStaus(SwapStatus newStatus)
        => Status = newStatus;

    // ------------------ Subswap logic ------------------

    public Result InitialBookReading(Guid userId, UserBook userBook)
    {
        if (SubSwapAccepting.UserId != userId)
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