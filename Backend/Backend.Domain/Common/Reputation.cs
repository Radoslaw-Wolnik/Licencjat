// Backend.Domain/Common/Reputation.cs
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Reputation(float Value)
{
    public static Reputation Initial() => new(4.0f);

    public static Result<Reputation> Create(float value)
    {
        if (value < 1 || value > 5)
            return Result.Fail("Reputation must be between 1 and 5");

        return new Reputation(value);
    }
}