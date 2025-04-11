using FluentResults;
using MediatR;

namespace Backend.Application.Features.Auth;

public sealed record ForgotCommand(
    string Email) : IRequest<Result<bool>>;
