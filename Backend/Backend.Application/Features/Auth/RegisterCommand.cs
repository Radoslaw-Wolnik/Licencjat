using FluentResults;
using MediatR;

namespace Backend.Application.Features.Auth;

public sealed record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName,
    DateTime BirthDate) : IRequest<Result<Guid>>;
