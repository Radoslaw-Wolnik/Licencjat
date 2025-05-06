using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Following;

public sealed record RemoveCommand(
    Guid FollowingLinkId
    ) : IRequest<Result>;