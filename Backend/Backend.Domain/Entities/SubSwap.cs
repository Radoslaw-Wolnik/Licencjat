// Backend.Domain/Entities/SubSwap.cs
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class SubSwap : Entity<Guid>
{
    public int PageAt { get; private set; }
    public bool IsRequesting { get; }
    public Guid UserId { get; }
    public Guid UserBookReadingId { get; }
    public Guid SwapId { get; }

    private SubSwap(Guid id, int pageAt, bool isRequesting, Guid userId, 
        Guid userBookReadingId, Guid swapId)
    {
        Id = id;
        PageAt = pageAt;
        IsRequesting = isRequesting;
        UserId = userId;
        UserBookReadingId = userBookReadingId;
        SwapId = swapId;
    }

    public static Result<SubSwap> Create(int pageAt, bool isRequesting, Guid userId, 
        Guid userBookReadingId, Guid swapId)
    {
        var errors = new List<IError>();
        
        if (pageAt < 0) errors.Add(SwapErrors.NegativePageNumber);
        if (userId == Guid.Empty) errors.Add(UserErrors.NotFound);
        if (userBookReadingId == Guid.Empty) errors.Add(BookErrors.NotFound);
        if (swapId == Guid.Empty) errors.Add(SwapErrors.NotFound);

        if (errors.Any()) return Result.Fail<SubSwap>(errors);

        return new SubSwap(
            Guid.NewGuid(),
            pageAt,
            isRequesting,
            userId,
            userBookReadingId,
            swapId
        );
    }

    public Result UpdateProgress(int newPage)
    {
        if (newPage < 0) return Result.Fail(SwapErrors.NegativePageNumber);
        PageAt = newPage;
        return Result.Ok();
    }
}