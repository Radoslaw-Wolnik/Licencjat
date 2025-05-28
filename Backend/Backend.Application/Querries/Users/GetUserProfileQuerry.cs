using Backend.Application.ReadModels.Users;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users;

public sealed record GetUserProfileQuerry(
    Guid UserId
    ) : IRequest<Result<UserProfileReadModel?>>;