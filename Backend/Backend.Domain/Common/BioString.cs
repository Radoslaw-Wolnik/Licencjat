using FluentResults;

namespace Backend.Domain.Common;

public sealed record BioString(string Value)
{
    public static BioString Initial() => new("");

    public static Result<BioString> Create(string value)
    {
        if (value.Length > 300)
            return Result.Fail("Bio must be max 300 characters");

        return new BioString(value);
    }
}