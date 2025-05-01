using FluentResults;

namespace Backend.Domain.Common;

public sealed record Photo(string Link)
{
    public string Link { get; } = Link;

    public static Result<Photo> Create(string link)
    {
        // validate the link
        
        return new Photo(link);
    }
}