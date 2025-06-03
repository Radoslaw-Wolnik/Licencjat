using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Common;

public sealed record Photo(string Link)
{
    public string Link { get; } = Link;

    public static Result<Photo> Create(string link)
    {
        // validate the link
        if (string.IsNullOrWhiteSpace(link)) 
            return Result.Fail(DomainErrorFactory.Invalid("Photo", "empty link"));
        
        return new Photo(link);
    }
}