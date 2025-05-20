using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Blocked;

public sealed record AddBlockedUserCommand(
    Guid UserId,
    Guid UserBlockedId
    ) : IRequest<Result>;