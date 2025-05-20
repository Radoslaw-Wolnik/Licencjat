using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Core;

public sealed record UpdateUserSensitiveInfoCommand(
    Guid UserId,
    string? FirstName,
    string? LastName
    ) : IRequest<Result<User>>;