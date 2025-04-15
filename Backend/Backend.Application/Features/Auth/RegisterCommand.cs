using FluentResults;
using MediatR;

namespace Backend.Application.Features.Auth;

public sealed record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string City,
    string Country) : IRequest<Result<Guid>>;
