using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed class SubSwap
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public int PageAt { get; private set; }

    public UserBook? UserBookReading { get; private set; }
    public Feedback? Feedback { get; private set; }
    public Issue? Issue { get; private set; }

    // initial possible user book but not necessary
    private SubSwap(
        Guid userId,
        UserBook? userBook
    ) : this (
        Guid.NewGuid(),
        userId,
        0,
        userBook,
        null,
        null
    ) { }

    // geenral constructor - usually userBook not null but the issue and feedback null
    private SubSwap (
        Guid id,
        Guid userId,
        int pageAt,
        UserBook? userBookReading,
        Feedback? feedback,
        Issue? issue
    ) {
        Id = id;
        UserId = userId;
        PageAt = pageAt;
        UserBookReading = userBookReading;
        Feedback = feedback;
        Issue = issue;
    }

    public static SubSwap Initial(Guid userId, UserBook? userBookReading){
        return new SubSwap(userId, userBookReading);
    }

    public static Result<SubSwap> Create(
        Guid id,
        Guid userId, 
        int pageAt,
        UserBook? userBookReading,
        Feedback? feedback,
        Issue? issue
        )
    {
        var errors = new List<IError>();
        
        if (pageAt <= 0) errors.Add(DomainErrorFactory.Invalid("SubSwap", "Page must be above 0 or equal 0"));
        if (userId == Guid.Empty) errors.Add(DomainErrorFactory.NotFound("User", userId));

        return errors.Count != 0
        ? Result.Fail<SubSwap>(errors)
        : new SubSwap(
            id,
            userId,
            pageAt,
            userBookReading,
            feedback,
            issue
        );
    }

    // Add Book but only if it was previously null
    public Result InitialBook (UserBook userBook){
        if (UserBookReading != null)
            return Result.Fail(DomainErrorFactory.Forbidden("Swap", "Cannot change the book during the swap"));
        
        UserBookReading = userBook;
        return Result.Ok();
    }

    // update the page at
    public Result UpdatePageAt (int newPageAt)
    {
        if (newPageAt < 0)
            return Result.Fail(DomainErrorFactory.Invalid("Swap", "page must be ≥ 0")); // nameof(SubSwap)


        if (UserBookReading == null) 
            return Result.Fail(DomainErrorFactory.Forbidden("Swap.BookAt.BookNull", "Cannot change the page if the book is null"));
        
        if(newPageAt > UserBookReading.PageCount)
            return Result.Fail(DomainErrorFactory.Invalid("Swap", "page must be lower or equal to the book total page count"));

        PageAt = newPageAt;

        return Result.Ok();
    }

    // Add Update Remove Issue
    public Result AddIssue(Issue issue){
        Issue = issue;
        return Result.Ok();
    }

    public Result UpdateIssue(Issue newIssue){
        if (Issue == null)
            return Result.Fail(DomainErrorFactory.Forbidden("Swap.Issue.Empty", "Cannot update issue that doesnt exsist"));
        
        if (Issue.UserId != newIssue.UserId || Issue.SubSwapId != newIssue.SubSwapId)
            return Result.Fail(DomainErrorFactory.Invalid("Swap.Issue", "Cannot change user or swap of an issue"));
        
        if (Issue.Description == newIssue.Description)
            return Result.Fail(DomainErrorFactory.Invalid("Swap.Issue", "Description is the same as before"));

        Issue = newIssue;

        return Result.Ok();
    }

    public void RemoveIssue()
        => Issue = null;

    // Add Update Feedback
    public Result AddFeedback(Feedback feedback){
        Feedback = feedback;
        return Result.Ok();
    }

    public Result UpdateFeedback(Feedback updatedFeedback){

        if (Feedback == null)
            return Result.Fail(DomainErrorFactory.Forbidden("Swap.Feedback.Empty", "Cannot update feedback that doesnt exsist"));

        if (updatedFeedback == Feedback)
            return Result.Fail(DomainErrorFactory.Invalid("Swap.Feedback", "Nothing wss changed in the feedback update"));
        
        if (Feedback.UserId != updatedFeedback.UserId || Feedback.SubSwapId != updatedFeedback.SubSwapId)
            return Result.Fail(DomainErrorFactory.Invalid("Swap.Feedback", "Cannot change user or swap of a feedback"));

        Feedback = updatedFeedback;

        return Result.Ok();
    }
        
}