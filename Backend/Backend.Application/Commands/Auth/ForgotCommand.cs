using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Auth;

public sealed record ForgotCommand(
    string Email) : IRequest<Result<bool>>;
