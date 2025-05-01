using FluentResults;

namespace Backend.Domain.Common;

public sealed record Rating(float Value)
{
    public static Rating Initial() => new(8.0f);

    public static Result<Rating> Create(float value)
    {
        if (value < 1 || value > 10)
            return Result.Fail("Rating must be between 1 and 10");

        return new Rating(value);
    }
}