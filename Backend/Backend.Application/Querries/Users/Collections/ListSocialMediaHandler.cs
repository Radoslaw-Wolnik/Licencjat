using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

// ListSocialMediaHandler.cs
public class ListSocialMediaHandler
    : IRequestHandler<ListSocialMediaQuery, Result<IReadOnlyCollection<SocialMediaLinkReadModel>>>
{
    private readonly IUserQueryService _userQuery;

    public ListSocialMediaHandler(IUserQueryService userQuery)
    {
        _userQuery = userQuery;
    }

    public async Task<Result<IReadOnlyCollection<SocialMediaLinkReadModel>>> Handle(
        ListSocialMediaQuery request,
        CancellationToken cancellationToken)
    {
        var socialMedia = await _userQuery.ListSocialMediaAsync(
            request.UserId, cancellationToken);

        return Result.Ok(socialMedia);
    }
}
