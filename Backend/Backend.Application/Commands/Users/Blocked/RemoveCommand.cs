using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Blocked;

public sealed record RemoveCommand(
    Guid BlockedLinkId
    ) : IRequest<Result>;