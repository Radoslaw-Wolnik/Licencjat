using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Common;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

public class GetSocialMediaByIdHandler
    : IRequestHandler<GetSocialMediaByIdQuerry, Result<SocialMediaLinkReadModel>>
{
    private readonly IUserQueryService _userQuery;

    public GetSocialMediaByIdHandler(IUserQueryService userQuery)
    {
        _userQuery = userQuery;
    }

    public async Task<Result<SocialMediaLinkReadModel>> Handle(
        GetSocialMediaByIdQuerry request,
        CancellationToken cancellationToken)
    {
        var socialMedia = await _userQuery.GetSocialMediaByIdAsync(
            request.SocialMediaId, cancellationToken);

        return socialMedia is null
            ? Result.Fail(DomainErrorFactory.NotFound("Social media link", request.SocialMediaId))
            : Result.Ok(socialMedia);
    }
}
