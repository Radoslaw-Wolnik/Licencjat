using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Core;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    string? City,
    string? CountryCode,
    string? Bio
    ) : IRequest<Result<User>>;