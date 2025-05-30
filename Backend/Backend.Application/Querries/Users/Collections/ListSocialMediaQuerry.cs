using Backend.Application.ReadModels.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

public sealed record ListSocialMediaQuery(
    Guid UserId
    ) : IRequest<Result<IReadOnlyCollection<SocialMediaLinkReadModel>>>;
    // bool Descending,
    // int Offset,
    // int Limit
    // ) : IRequest<Result<PaginatedResult<UserSmallReadModel>>>;
// we will just list all of them - no offset and pagination needed
