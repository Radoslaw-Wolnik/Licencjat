using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Core;

public sealed record DeleteCommand(
    Guid UserId
    ) : IRequest<Result>;