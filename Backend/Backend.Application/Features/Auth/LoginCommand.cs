using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Features.Auth;

public sealed record LoginCommand(
    string UsernameOrEmail,
    string Password,
    bool? RememberMe) : IRequest<Result<User>>;
